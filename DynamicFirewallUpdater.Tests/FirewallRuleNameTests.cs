using ACSSolutions.DynamicFirewallUpdater.Settings;

namespace ACSSolutions.DynamicFirewallUpdater.Tests
{
	/// <summary>
	/// The rule name is the identity the run matches on to decide between updating an
	/// existing firewall entry and creating a new one. If its format ever drifts, every
	/// machine starts adding a second rule instead of maintaining its own.
	/// </summary>
	public class FirewallRuleNameTests
	{

		[Fact]
		public void Is_Scoped_To_This_Machine_And_Stable_Across_Instances()
		{
			var first = NewProgram().FirewallRuleName;
			var second = NewProgram().FirewallRuleName;

			Assert.Equal( first, second );
			Assert.Contains( Environment.MachineName, first );
			Assert.Equal( $"{Environment.MachineName} dynamic update", first );
		}

		[Fact]
		public void Is_A_Legal_Azure_Sql_Firewall_Rule_Name()
		{
			// Azure rejects '<', '>', '%', '&', ':', ';', '\' and a trailing period in a
			// firewall rule name, and caps it at 128 characters.
			var name = NewProgram().FirewallRuleName;

			Assert.NotEmpty( name );
			Assert.InRange( name.Length, 1, 128 );
			Assert.DoesNotContain( '<', name );
			Assert.DoesNotContain( '>', name );
			Assert.DoesNotContain( '%', name );
			Assert.DoesNotContain( '&', name );
			Assert.DoesNotContain( ':', name );
			Assert.DoesNotContain( ';', name );
			Assert.DoesNotContain( '\\', name );
			Assert.False( name.EndsWith( '.' ) );
		}

		private static Program NewProgram()
			=> new Program(
				new Settings.Settings
				{
					CGNAT = new CGNAT(),
					Azure = new Settings.Azure { Directories = new List<Settings.Directory>() },
				}
			);

	}
}
