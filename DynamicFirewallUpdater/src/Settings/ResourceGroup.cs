
using System;
using System.Collections.Generic;
using System.Text;

namespace ACSSolutions.DynamicFirewallUpdater.Settings
{
	public class ResourceGroup
	{
		public String Name { get; set; } = null!;
		public List<String> SqlServers { get; set; } = null!;
	}
}
