# 01-upgrade-dynamicfirewallupdater: Upgrade project to .NET 10

Update `DynamicFirewallUpdater.csproj` to target `net10.0`. All 17 NuGet packages are already compatible with .NET 10 and do not require version changes.

One breaking API change must be fixed: `ConfigurationBinder.Get<T>(IConfiguration)` at `Program.cs` line 122 is binary incompatible in .NET 10. Research the replacement API (likely `GetRequiredSection` + `Get<T>()` on the section, or using `Bind`) and apply the fix inline.

Additionally, enable nullable reference types by adding `<Nullable>enable</Nullable>` to the project file, then address any resulting compile-time null warnings across the codebase.

**Done when**: `DynamicFirewallUpdater.csproj` targets `net10.0`, nullable is enabled (`<Nullable>enable</Nullable>`), the `ConfigurationBinder.Get<T>` call is replaced with a .NET 10-compatible alternative, and the project builds with 0 errors and 0 warnings.
