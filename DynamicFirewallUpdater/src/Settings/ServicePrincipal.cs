using System;

namespace ACSSolutions.DynamicFirewallUpdater.Settings
{
	public class ServicePrincipal
	{
		public Guid Id { get; set; }
		public String Secret { get; set; }
		public String CertificateThumbprint { get; set; }
	}
}
