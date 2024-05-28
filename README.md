# Dynamic SQL Azure Firewall Updater

This project is intended toi help the itinerant developer who need to connect to an Azure SQL Database from multiple locations and/or dynamic IP addresses

The basic idea is to have just one entry for your PC on the firewalls of all the Azure SQL Servers to which you need to connect, and to run a scheduled task frequently to use the API to update the IP address of the firewall rule in necessary.

## Setup in Azure

In the [Azure Portal](https://portal.azure.com/) go to the [Entra ID blade](https://portal.azure.com/#view/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/~/RegisteredApps).

1. Create a new App. I called mine Dynamic Firewall Updater.
1. Go to Certificates & Secrets, and create one or the other for each user or PC that will run this software. Certificates are better, and there is a PowerShell script in the root of the project to help you create one.
1. Grant permissions. I usually go to the individual Azure SQL Server, but you can apply permissions ath the Resopurce Group or even Subscription levels if required. In Access Control (IAM), grant membership of the "SQL Security Manager" role to your new app identity.

## Deployment
I put the binaries in "C:\Program Files\ACS Solutions\DynamicFirewallUpdater\bin".

## Configuration
The app reads configuration from appsettings.json in its own folder, and also from ..\config\appsettings.json, so you can keep your configuration away from the binary deployment folder.
Review the template.appsettings.json in the Config folder, and then create your own, plugging in all the guids and names from the Azure Portal.

## Prerequisites
You'll need the .NET 8.0 Runtime installed. See https://dotnet.microsoft.com/en-us/download

## Logging
The app writes to a log file, but default in %TEMP%. The configuration is in appSettings.json and uses the [Serilog](https://serilog.net/) project so you can follow their guidance if you need to change anything.

## First run
Run DynamicFirewallUpdater.exe from the command-line.

## Scheduling
There's an example scheduled job saved as XML in the root of the project called "Update Azure Firewalls Scheduled Task.xml". Open task scheduler and load that up and it should do the trick.

## Support
Raise issues on here if you need help!