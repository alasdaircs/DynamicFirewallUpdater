using Azure.Identity;

using ACSSolutions.DynamicFirewallUpdater.Settings;

namespace ACSSolutions.DynamicFirewallUpdater.Tests
{
	/// <summary>
	/// Which credential a directory builds, and the order it prefers them in. None of
	/// these tests reach Azure: they assert on the credential object that is constructed,
	/// or on the exception raised before any network call happens.
	/// </summary>
	public class CredentialSelectionTests
	{

		private static readonly Guid TenantId = Guid.Parse( "11111111-1111-1111-1111-111111111111" );
		private static readonly Guid ClientId = Guid.Parse( "22222222-2222-2222-2222-222222222222" );

		[Fact]
		public void A_Secret_Yields_A_ClientSecretCredential()
		{
			var directory = DirectoryWith( new ServicePrincipal { Id = ClientId, Secret = "a-secret" } );

			Assert.IsType<ClientSecretCredential>( directory.GetCredential( TenantId ) );
		}

		[Theory]
		[InlineData( null )]
		[InlineData( "" )]
		[InlineData( "   " )]
		public void Neither_A_Secret_Nor_A_Thumbprint_Is_An_Error( String? blank )
		{
			// Falling through to a default or managed identity here would be worse than
			// failing: the run would authenticate as something other than the configured
			// service principal.
			var directory = DirectoryWith(
				new ServicePrincipal { Id = ClientId, Secret = blank, CertificateThumbprint = blank }
			);

			var exception = Assert.Throws<CredentialUnavailableException>(
				() => directory.GetCredential( TenantId )
			);
			Assert.Contains( "CertificateThumbprint", exception.Message );
		}

		[Fact]
		public void A_Thumbprint_Takes_Precedence_And_Does_Not_Fall_Back_To_The_Secret()
		{
			// Windows only. GetCredential opens the LocalMachine store unguarded, and on
			// Unix that throws PlatformNotSupportedException ("limited to the Root and
			// CertificateAuthority stores") before it can report the certificate missing.
			// The app is deployed as a Windows scheduled task, so CI runs this for real;
			// this skip only keeps the suite green for anyone working on Linux or macOS.
			Assert.SkipUnless( OperatingSystem.IsWindows(), "X509Store LocalMachine is Windows-only" );

			// A thumbprint that resolves to no certificate must fail loudly. Silently using
			// the secret instead would mask a broken certificate deployment indefinitely.
			var directory = DirectoryWith(
				new ServicePrincipal
				{
					Id = ClientId,
					Secret = "a-secret",
					CertificateThumbprint = "0000000000000000000000000000000000000000",
				}
			);

			var exception = Assert.Throws<CredentialUnavailableException>(
				() => directory.GetCredential( TenantId )
			);
			Assert.Contains( "not found", exception.Message );
		}

		private static Settings.Directory DirectoryWith( ServicePrincipal principal )
			=> new Settings.Directory
			{
				Id = TenantId,
				ServicePrincipal = principal,
				Subscriptions = new List<Subscription>(),
			};

	}
}
