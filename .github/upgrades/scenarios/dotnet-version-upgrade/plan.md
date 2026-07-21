# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade DynamicFirewallUpdater from net9.0 to net10.0
**Scope**: 1 project, ~17 NuGet packages (all compatible), 1 breaking API usage, nullable reference types to be enabled

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 1 project, already on modern .NET, clear upgrade path with no dependency graph complexity.

## Tasks

### 01-upgrade-dynamicfirewallupdater: Upgrade project to .NET 10

Update `DynamicFirewallUpdater.csproj` to target `net10.0`. All 17 NuGet packages are already compatible with .NET 10 and do not require version changes.

One breaking API change must be fixed: `ConfigurationBinder.Get<T>(IConfiguration)` at `Program.cs` line 122 is binary incompatible in .NET 10. Research the replacement API (likely `GetRequiredSection` + `Get<T>()` on the section, or using `Bind`) and apply the fix inline.

Additionally, enable nullable reference types by adding `<Nullable>enable</Nullable>` to the project file, then address any resulting compile-time null warnings across the codebase.

**Done when**: `DynamicFirewallUpdater.csproj` targets `net10.0`, nullable is enabled (`<Nullable>enable</Nullable>`), the `ConfigurationBinder.Get<T>` call is replaced with a .NET 10-compatible alternative, and the project builds with 0 errors and 0 warnings.

---

### 02-validate: Final validation

Perform a full solution build and run all tests to confirm the upgrade is complete and nothing has regressed. Verify the binary runs correctly and document any deferred recommendations.

**Done when**: Solution builds successfully with 0 errors, all tests pass, and no unresolved issues remain.
