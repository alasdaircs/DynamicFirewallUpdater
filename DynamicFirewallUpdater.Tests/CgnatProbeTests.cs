namespace ACSSolutions.DynamicFirewallUpdater.Tests
{
	/// <summary>
	/// <see cref="Program.IsBehindCgnat"/> swallows the exceptions that mean "ICMP is
	/// unavailable on this host" so an unprivileged Linux box does not fail the whole run.
	/// These tests pin that the filter stays narrow: a swallow-everything catch here would
	/// turn a genuine fault into a silent decision not to widen the firewall.
	/// </summary>
	public class CgnatProbeTests
	{

		[Theory]
		[InlineData( "not-an-ip" )]
		[InlineData( "" )]
		public void A_Malformed_Address_Still_Propagates( String ip )
		{
			// IPAddress.Parse throws inside the try block. FormatException is not in the
			// catch filter, so a bad address must surface rather than be reported as
			// "no CGNAT" — otherwise a typo in config would look like a successful probe.
			Assert.Throws<FormatException>( () => Program.IsBehindCgnat( ip, 2 ) );
		}

	}
}
