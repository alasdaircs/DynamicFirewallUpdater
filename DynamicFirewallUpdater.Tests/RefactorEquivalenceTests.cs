using System.Net;

using ACSSolutions.DynamicFirewallUpdater.Settings;

namespace ACSSolutions.DynamicFirewallUpdater.Tests
{
	/// <summary>
	/// Characterisation test for the extraction of Program.CgnatRange out of Run().
	/// Original() below is the pre-refactor body copied verbatim from 0df5438, including
	/// its field-access path through Settings, so this compares the old statements against
	/// the new method rather than comparing the new method to itself.
	/// </summary>
	public class RefactorEquivalenceTests
	{

		/// <summary>Verbatim from Program.Run at 0df5438, lines 195-198.</summary>
		private static ( String Start, String End ) Original( String ip, Byte subnetLength )
		{
			var _settings = new Settings.Settings
			{
				CGNAT = new CGNAT { SubnetLength = subnetLength, TTL = 2 },
				Azure = new Settings.Azure { Directories = new List<Settings.Directory>() },
			};
			var ipStart = ip;
			var ipEnd = ip;

			var host = IPAddress.Parse( ipStart );
			var network = IPNetwork2.Parse( host.ToString() + "/" + _settings.CGNAT.SubnetLength.ToString() );
			ipStart = network.FirstUsable.ToString();
			ipEnd = network.LastUsable.ToString();

			return ( ipStart, ipEnd );
		}

		[Fact]
		public void CgnatRange_Agrees_With_The_Pre_Refactor_Implementation()
		{
			String[] addresses =
			{
				"203.0.113.37",      // documentation range, mid-octet
				"100.64.0.1",        // the actual CGNAT shared address space
				"100.127.255.254",   // top of that space
				"10.0.0.1",          // private, lowest host
				"192.168.1.255",     // host octet at 255
				"172.16.0.0",        // host octet at 0
				"198.51.100.200",
				"1.0.0.1",           // lowest routable
				"255.255.255.254",   // highest host
				"8.8.8.8",
			};

			var compared = 0;

			foreach( var address in addresses )
			{
				// Every subnet length the Range attribute permits.
				for( Byte subnetLength = 16; subnetLength <= 32; subnetLength++ )
				{
					var expected = Original( address, subnetLength );
					var actual = Program.CgnatRange( address, subnetLength );

					Assert.Equal( expected.Start, actual.Start );
					Assert.Equal( expected.End, actual.End );
					compared++;
				}
			}

			Assert.Equal( addresses.Length * 17, compared );
		}

		[Fact]
		public void Both_Implementations_Agree_On_Unusual_Input()
		{
			// Asserts agreement, not validity. Note "1.2.3" is not rejected: .NET reads
			// legacy partial dotted-quad notation and parses it as 1.2.0.3. Both the old
			// and new code inherit that, which is the point — equivalence either way.
			foreach( var input in new[] { "not-an-ip", "", "256.1.1.1", "1.2.3", " 8.8.8.8 " } )
			{
				( String Start, String End )? originalResult = null;
				( String Start, String End )? actualResult = null;

				var originalThrew = Record.Exception( () => originalResult = Original( input, 24 ) );
				var actualThrew = Record.Exception( () => actualResult = Program.CgnatRange( input, 24 ) );

				if( originalThrew is null )
				{
					Assert.Null( actualThrew );
					Assert.Equal( originalResult, actualResult );
				}
				else
				{
					Assert.NotNull( actualThrew );
					Assert.Equal( originalThrew.GetType(), actualThrew.GetType() );
					Assert.Equal( originalThrew.Message, actualThrew.Message );
				}
			}
		}

	}
}
