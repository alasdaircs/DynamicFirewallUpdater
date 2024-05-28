using System;
using System.Collections.Generic;
using System.Text;

namespace ACSSolutions.DynamicFirewallUpdater.Settings
{
	public class Subscription
	{
		public Guid Id { get; set; }
		public List<ResourceGroup> ResourceGroups { get; set; }
	}
}
