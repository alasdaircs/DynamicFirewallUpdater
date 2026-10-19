using Microsoft.Extensions.Configuration;

namespace ACSSolutions.DynamicFirewallUpdater.Tests
{
	/// <summary>
	/// appsettings.json points the file sink at "%TEMP%\DynamicFirewallUpdater-.log".
	/// Windows expands that; Unix does not, and without rewriting it Serilog creates a file
	/// named literally "%TEMP%\..." in whatever directory the app was started from.
	/// </summary>
	public class LogPathTests
	{

		[Fact]
		public void A_Windows_Temp_Path_Is_Rewritten_Under_The_Platform_Temp_Directory()
		{
			var resolved = Resolve( "%TEMP%\\DynamicFirewallUpdater-.log" );

			if( OperatingSystem.IsWindows() )
			{
				// Windows understands the original, so there is nothing to override.
				Assert.Empty( resolved );
				return;
			}

			var ( key, value ) = Assert.Single( resolved );
			Assert.Equal( "Serilog:WriteTo:0:Args:path", key );
			Assert.NotNull( value );
			Assert.StartsWith( Path.GetTempPath(), value );
			Assert.EndsWith( "DynamicFirewallUpdater-.log", value );
			Assert.DoesNotContain( "%TEMP%", value );
			Assert.DoesNotContain( '\\', value );
			Assert.True( Path.IsPathRooted( value ) );
		}

		[Fact]
		public void Paths_Without_A_Windows_Variable_Are_Left_Alone()
		{
			// An operator who has already configured an absolute Unix path must keep it.
			Assert.Empty( Resolve( "/var/log/dynamicfirewallupdater-.log" ) );
		}

		[Fact]
		public void Non_Path_Serilog_Settings_Are_Not_Touched()
		{
			var configuration = new ConfigurationBuilder()
				.AddInMemoryCollection( new Dictionary<String, String?>
				{
					["Serilog:MinimumLevel"] = "Debug",
					["Serilog:WriteTo:0:Name"] = "Console",
					["Serilog:WriteTo:0:Args:rollingInterval"] = "Day",
				} )
				.Build();

			Assert.Empty( Program.ResolveWindowsLogPaths( configuration ) );
		}

		private static Dictionary<String, String?> Resolve( String path )
		{
			var configuration = new ConfigurationBuilder()
				.AddInMemoryCollection( new Dictionary<String, String?>
				{
					["Serilog:WriteTo:0:Name"] = "File",
					["Serilog:WriteTo:0:Args:path"] = path,
				} )
				.Build();

			return Program.ResolveWindowsLogPaths( configuration );
		}

	}
}
