# .NET Version Upgrade

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0

## Strategy
**Selected**: All-at-Once
**Rationale**: Single project on net9.0, all packages compatible, straightforward TFM bump.

### Execution Constraints
- Single atomic upgrade — all changes applied in one pass
- Validate full solution build after upgrade before committing
- Fix ConfigurationBinder.Get<T> API change inline (no stubs)
- Enable nullable reference types and resolve all warnings in the same task

## Upgrade Options
**Source**: .github/upgrades/scenarios/dotnet-version-upgrade/upgrade-options.md

### Strategy
- Upgrade Strategy: All-at-Once

### Compatibility
- Unsupported API Handling: Fix Inline

### Modernization
- Nullable Reference Types: Enable Nullable Reference Types

## Source Control
- **Source Branch**: main
- **Commit Strategy**: Single Commit at End
- **Branch Sync**: Disabled
