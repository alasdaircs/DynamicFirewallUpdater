using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;

using Azure.Core;
using Azure.Identity;

namespace ACSSolutions.DynamicFirewallUpdater.Settings
{
	public class Directory
	{
		public Guid Id { get; set; }
		public ServicePrincipal ServicePrincipal { get; set; } = null!;
		public List<Subscription> Subscriptions { get; set; } = null!;

		public TokenCredential GetCredential( Guid tenantId )
		{
			TokenCredential result;

			if( !String.IsNullOrWhiteSpace( ServicePrincipal.CertificateThumbprint ) )
			{
				X509Certificate2? certificate = null;

				// On Unix the LocalMachine store only exposes Root and CertificateAuthority,
				// so opening it for a client certificate throws PlatformNotSupportedException
				// rather than simply finding nothing. CurrentUser is the only usable store
				// off Windows.
				var storeLocations = OperatingSystem.IsWindows()
					? new[] { StoreLocation.CurrentUser, StoreLocation.LocalMachine }
					: new[] { StoreLocation.CurrentUser };

				foreach( var storeLocation in storeLocations )
				{
					X509Store store = new X509Store( storeLocation );
					store.Open( OpenFlags.ReadOnly );
					X509Certificate2Collection certs = store.Certificates.Find( X509FindType.FindByThumbprint, ServicePrincipal.CertificateThumbprint, false ); // false is required if using self-signed certs
					store.Close();

					if( certs.Count == 1 )
					{
						certificate = certs[0];
						break;
					}
				}

				if( certificate == null )
				{
					throw new CredentialUnavailableException( $"Certificate with thumbprint {ServicePrincipal.CertificateThumbprint} not found" );
				};

				result = new ClientCertificateCredential(
					tenantId.ToString(),
					ServicePrincipal.Id.ToString(),
					certificate
				);
			}
			else if( !String.IsNullOrWhiteSpace( ServicePrincipal.Secret ) )
			{
				result = new ClientSecretCredential(
					Id.ToString(),
					ServicePrincipal.Id.ToString(),
					ServicePrincipal.Secret!
				);
			}
			else
			{
				throw new CredentialUnavailableException( "You must supply either a CertificateThumbprint or a Secret" );
			}

			return result;
		}
	}
}
