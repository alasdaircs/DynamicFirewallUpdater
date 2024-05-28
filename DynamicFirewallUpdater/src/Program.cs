using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Azure.ResourceManager;
using Azure.ResourceManager.Sql;

using Flurl;
using Flurl.Http;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

using Serilog;

namespace ACSSolutions.DynamicFirewallUpdater
{

	class Program
	{

		private static class AppSettings
		{

			public const String DefaultsFile
				= "appsettings.json";

			public const String OverlayFileName
				= "appsettings.json";

			public static String OverlayFilePath
			{
				get
				{
					var segments = Environment.CurrentDirectory.Split( Path.DirectorySeparatorChar  );
					Int32 binpos = segments.Length - 1;

					for( int i = segments.Length - 1; i >= 0; i-- )
					{
						if( StringComparer.InvariantCultureIgnoreCase.Equals( segments[i], "bin" ) )
						{
							binpos = i;
							break;
						}
					}

					var parentpath = String.Join( Path.DirectorySeparatorChar, segments.Take( binpos ) );
					// var parentpath = string.Join( Path.DirectorySeparatorChar, Enumerable.Repeat( "..", segments.Length - binpos ) );

					return
						Path.Combine(
							parentpath,
							"config"
						);
				}
			}

			public static String OverlayFile
				= Path.Combine(
						OverlayFilePath,
						OverlayFileName
					);
		}

		protected Settings.Azure _settings { get; }

		public string FirewallRuleName
		{
			get => $"{Environment.MachineName} dynamic update";
		}


		static void Main( string[] args )
		{
			var configurationBuilder = new ConfigurationBuilder()
				.AddJsonFile( AppSettings.DefaultsFile )
				.AddJsonFile( AppSettings.OverlayFile, optional: true )
				.AddEnvironmentVariables();

#if DEBUG
			configurationBuilder
				.AddUserSecrets<Program>();
#endif

			configurationBuilder
				.AddCommandLine( args );

			var configuration = configurationBuilder.Build();

			Log.Logger = new LoggerConfiguration()
				.ReadFrom.Configuration( configuration )
				.CreateLogger();

			Log.Information( "Started in {workingDirectory}", Environment.CurrentDirectory );

			#if DEBUG

			var files = configurationBuilder.GetFileProvider();
			Log.Verbose(
				"{AppSettingsDefaultsFile}: {AppSettingsDefaultsFileExists}",
				AppSettings.DefaultsFile,
				files.GetFileInfo( AppSettings.DefaultsFile ).Exists
			);
			Log.Verbose(
				"{AppSettingsOverlayFile}: {AppSettingsOverlayFileExists}",
				AppSettings.OverlayFile,
				new PhysicalFileProvider( AppSettings.OverlayFilePath ).GetFileInfo( AppSettings.OverlayFileName ).Exists
			);

			foreach( var entry in configuration.AsEnumerable().OrderBy( pair => pair.Key ) )
			{
				Log.Verbose( "{config} = {value}", entry.Key, entry.Value );
			}
			#endif

			var azureSettings = configuration.GetSection( "Azure" ).Get<Settings.Azure>();
			var cancellation = new CancellationTokenSource();
			Console.CancelKeyPress += ( sender, cancel_args ) => {
				if (!cancellation.IsCancellationRequested) {
					cancel_args.Cancel = true;  // prevent process quit (first time)
					cancellation.Cancel();
				}
			};
			try {
				new Program( azureSettings )
					.Run( cancellation.Token )
					.Wait();
			}
			catch (OperationCanceledException) {
				Log.Warning( "Cancelled" );
			}
			catch (Exception ex) {
				Log.Error( ex, "Execution failed" );
			}
			Log.Information( "Finished" );
		}

		public Program( Settings.Azure settings )
		{
			_settings = settings;
		}

		private async Task Run( CancellationToken cancellation )
		{

			var ip = await "https://api.ipify.org".GetStringAsync( cancellationToken: cancellation );
			Log.Debug( "Current IP {0}", ip );

			foreach( var directory in _settings.Directories.Where( d => d.Id != Guid.Empty ) )
			{
				Log.Debug( "Directory {0}", directory.Id );

				var client = new ArmClient(
					directory.GetCredential( directory.Id )
				);

				foreach( var subscription in directory.Subscriptions )
				{
					Log.Debug( "Subscription {0}", subscription.Id );

					foreach( var resourceGroup in subscription.ResourceGroups )
					{
						Log.Debug( "ResourceGroup {0}", resourceGroup.Name );

						foreach( var sqlServerName in resourceGroup.SqlServers )
						{
							Log.Debug( "SQL Server {0}", sqlServerName );

							var sqlServerResource = client.GetSqlServerResource(
								SqlServerResource.CreateResourceIdentifier(
									subscription.Id.ToString(),
									resourceGroup.Name,
									sqlServerName
								)
							);

							var firewallRules = sqlServerResource.GetSqlFirewallRules();

							var firewallRuleExists = (
								await firewallRules.ExistsAsync(
									FirewallRuleName,
									cancellation
								)
							).Value;

							if( firewallRuleExists )
							{
								var firewallRule = (
									await firewallRules.GetAsync(
										FirewallRuleName,
										cancellation
									)
								).Value;

								if( firewallRule.Data.StartIPAddress == ip )
								{
									Log.Debug( "IP Address not changed for server {0}.{1}", resourceGroup.Name, sqlServerName );
								}
								else
								{
									Log.Information( "IP Address changed from {0} to {1} - updating {2}.{3}", firewallRule.Data.StartIPAddress, ip, resourceGroup.Name, sqlServerName );
									await firewallRule.UpdateAsync(
										Azure.WaitUntil.Completed,
										new SqlFirewallRuleData()
										{
											StartIPAddress = ip,
											EndIPAddress = ip
										},
										cancellation
									);
									Log.Debug( "IP Address updated" );
								}
							}
							else
							{
								Log.Information( "Firewall entry {0} not found in {1}.{2} - creating new one", FirewallRuleName, resourceGroup.Name, sqlServerName );
								await firewallRules.CreateOrUpdateAsync(
									Azure.WaitUntil.Completed,
									FirewallRuleName,
									new SqlFirewallRuleData()
									{
										Name = FirewallRuleName,
										StartIPAddress = ip,
										EndIPAddress = ip
									},
									cancellation
								);
								Log.Debug( "Firewall entry created" );
							}

						}
					}
				}
			}
		}

	}

}
