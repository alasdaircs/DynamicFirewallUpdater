using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ACSSolutions.DynamicFirewallUpdater.Settings;

namespace ACSSolutions.DynamicFirewallUpdater.Settings
{
	public class Settings
	{
		public CGNAT CGNAT { get; set; }
		public Azure Azure { get; set; }
	}
}
