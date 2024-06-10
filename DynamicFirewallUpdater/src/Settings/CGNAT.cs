using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACSSolutions.DynamicFirewallUpdater.Settings
{
	public class CGNAT
	{
		[Range(16,32)]
		public Byte SubnetLength { get; set; } = 24;
		[Range(1,16)]
		public Byte TTL { get; set; } = 2;
	}
}
