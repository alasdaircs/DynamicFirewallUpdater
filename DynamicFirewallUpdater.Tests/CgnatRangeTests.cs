using System.Net.Sockets;

namespace ACSSolutions.DynamicFirewallUpdater.Tests
{
	/// <summary>
	/// <see cref="Program.CgnatRange"/> decides which addresses the firewall entry is
	/// widened to cover, so a regression here silently opens the wrong range.
	/// </summary>
	public class CgnatRangeTests
	{

		[Theory]
		// The configured default. /24 keeps the whole last octet, minus network and broadcast.
		[InlineData( "203.0.113.37", (Byte)24, "203.0.113.1", "203.0.113.254" )]
		// Narrower than the host address: the range must not escape the parent subnet.
		[InlineData( "203.0.113.37", (Byte)28, "203.0.113.33", "203.0.113.46" )]
		// The widest the Range attribute permits.
		[InlineData( "203.0.113.37", (Byte)16, "203.0.0.1", "203.0.255.254" )]
		public void Expands_To_The_Usable_Range_Of_The_Containing_Subnet(
			String ip,
			Byte subnetLength,
			String expectedStart,
			String expectedEnd
		)
		{
			var ( start, end ) = Program.CgnatRange( ip, subnetLength );

			Assert.Equal( expectedStart, start );
			Assert.Equal( expectedEnd, end );
		}

		[Fact]
		public void A_Single_Host_Subnet_Covers_Only_That_Host()
		{
			// /32 is the other end of the permitted Range. It must stay a single address:
			// widening a non-CGNAT address to a subnet would open the firewall unnecessarily.
			var ( start, end ) = Program.CgnatRange( "203.0.113.37", 32 );

			Assert.Equal( "203.0.113.37", start );
			Assert.Equal( "203.0.113.37", end );
		}

		[Fact]
		public void The_Range_Always_Contains_The_Observed_Address()
		{
			// Whatever the subnet length, the address we actually connect from has to fall
			// inside the range we open, or the rule we just wrote locks us out.
			var observed = System.Net.IPAddress.Parse( "198.51.100.200" );

			foreach( Byte subnetLength in new Byte[] { 16, 20, 24, 28, 32 } )
			{
				var ( start, end ) = Program.CgnatRange( observed.ToString(), subnetLength );

				Assert.True(
					ToUInt32( start ) <= ToUInt32( observed.ToString() ),
					$"/{subnetLength}: start {start} is above the observed address"
				);
				Assert.True(
					ToUInt32( end ) >= ToUInt32( observed.ToString() ),
					$"/{subnetLength}: end {end} is below the observed address"
				);
			}
		}

		[Fact]
		public void A_Malformed_Address_Is_Rejected_Rather_Than_Guessed()
		{
			Assert.Throws<FormatException>( () => Program.CgnatRange( "not-an-ip", 24 ) );
		}

		private static UInt32 ToUInt32( String ip )
		{
			var octets = System.Net.IPAddress.Parse( ip ).GetAddressBytes();
			Assert.Equal( AddressFamily.InterNetwork, System.Net.IPAddress.Parse( ip ).AddressFamily );
			return ( (UInt32)octets[0] << 24 ) | ( (UInt32)octets[1] << 16 ) | ( (UInt32)octets[2] << 8 ) | octets[3];
		}

	}
}
