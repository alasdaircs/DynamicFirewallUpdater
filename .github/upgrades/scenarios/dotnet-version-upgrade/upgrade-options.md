# Upgrade Options — DynamicFirewallUpdater

Assessment: 1 project (net9.0 → net10.0), 17 compatible packages, 1 binary-incompatible API usage (ConfigurationBinder.Get<T>)

## Strategy

### Upgrade Strategy
Single project with no dependency graph to manage; all-at-once is the optimal approach.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade the single project in one pass — fastest approach with no multi-targeting overhead. |
| Top-Down | Upgrade entry-point app first, multi-targeting shared libraries temporarily. Not needed for a single-project solution. |

## Compatibility

### Unsupported API Handling
`ConfigurationBinder.Get<T>(IConfiguration)` is flagged as binary incompatible in .NET 10 (line 122 of Program.cs).

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve the API change in the same upgrade task — a known replacement exists for this ConfigurationBinder overload. No stubs or deferred work. |
| Defer Complex Changes | Generate a minimal compilable stub and create a follow-up resolution subtask. Not needed here — the change is straightforward. |

## Modernization

### Nullable Reference Types
Nullable reference types are not yet enabled; target is net10.0 which supports them.

| Value | Description |
|-------|-------------|
| Leave Disabled | Does not enable nullable. Maintains existing null handling. Can be enabled separately as a distinct effort after migration. |
| **Enable Nullable Reference Types** (selected) | Adds `<Nullable>enable</Nullable>` to the project file. Requires addressing compile-time null warnings. |
