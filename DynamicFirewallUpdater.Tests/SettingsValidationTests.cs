using System.ComponentModel.DataAnnotations;

using RecursiveDataAnnotationsValidation;

using ACSSolutions.DynamicFirewallUpdater.Settings;

namespace ACSSolutions.DynamicFirewallUpdater.Tests
{
	/// <summary>
	/// Startup validation is recursive on purpose: every Range attribute in the settings
	/// model sits on a nested object, so the framework's own TryValidateObject would not
	/// reach any of them. These tests pin that recursion, because a validator that quietly
	/// stops descending still compiles and still reports every config as valid.
	/// </summary>
	public class SettingsValidationTests
	{

		[Fact]
		public void A_Valid_Configuration_Passes()
		{
			var results = new List<ValidationResult>();

			Assert.True( Validate( SettingsWith( subnetLength: 24, ttl: 2 ), results ) );
			Assert.Empty( results );
		}

		[Theory]
		[InlineData( (Byte)15 )]  // just below the permitted range
		[InlineData( (Byte)0 )]
		public void A_SubnetLength_Below_The_Permitted_Range_Is_Rejected( Byte subnetLength )
		{
			var results = new List<ValidationResult>();

			Assert.False( Validate( SettingsWith( subnetLength, ttl: 2 ), results ) );
			Assert.Contains( results, r => r.MemberNames.Any( m => m.Contains( nameof( CGNAT.SubnetLength ) ) ) );
		}

		[Fact]
		public void A_SubnetLength_Above_The_Permitted_Range_Is_Rejected()
		{
			// 33 cannot describe an IPv4 subnet, and IPNetwork2.Parse would throw at the
			// point of use rather than at startup.
			var results = new List<ValidationResult>();

			Assert.False( Validate( SettingsWith( subnetLength: 33, ttl: 2 ), results ) );
			Assert.NotEmpty( results );
		}

		[Theory]
		[InlineData( (Byte)0 )]
		[InlineData( (Byte)17 )]
		public void A_Ttl_Outside_The_Permitted_Range_Is_Rejected( Byte ttl )
		{
			var results = new List<ValidationResult>();

			Assert.False( Validate( SettingsWith( subnetLength: 24, ttl ), results ) );
			Assert.Contains( results, r => r.MemberNames.Any( m => m.Contains( nameof( CGNAT.TTL ) ) ) );
		}

		[Fact]
		public void Both_Boundaries_Of_Each_Range_Are_Accepted()
		{
			// Guards against an off-by-one in the attributes themselves.
			Assert.True( Validate( SettingsWith( subnetLength: 16, ttl: 1 ), new List<ValidationResult>() ) );
			Assert.True( Validate( SettingsWith( subnetLength: 32, ttl: 16 ), new List<ValidationResult>() ) );
		}

		[Fact]
		public void The_Model_Defaults_Are_Themselves_Valid()
		{
			// appsettings.json may omit the CGNAT section entirely, in which case the
			// property initialisers are what the app runs with.
			var settings = new Settings.Settings
			{
				CGNAT = new CGNAT(),
				Azure = new Settings.Azure { Directories = new List<Settings.Directory>() },
			};

			Assert.Equal( 24, settings.CGNAT.SubnetLength );
			Assert.Equal( 2, settings.CGNAT.TTL );
			Assert.True( Validate( settings, new List<ValidationResult>() ) );
		}

		private static Settings.Settings SettingsWith( Byte subnetLength, Byte ttl )
			=> new Settings.Settings
			{
				CGNAT = new CGNAT { SubnetLength = subnetLength, TTL = ttl },
				Azure = new Settings.Azure { Directories = new List<Settings.Directory>() },
			};

		private static Boolean Validate( Settings.Settings settings, List<ValidationResult> results )
			=> new RecursiveDataAnnotationValidator()
				.TryValidateObjectRecursive( settings, new ValidationContext( settings ), results );

	}
}
