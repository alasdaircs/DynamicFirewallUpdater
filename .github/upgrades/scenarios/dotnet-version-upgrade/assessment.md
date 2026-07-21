# Projects and dependencies analysis

This document provides a comprehensive overview of the projects and their dependencies in the context of upgrading to .NETCoreApp,Version=v10.0.

## Table of Contents

- [Executive Summary](#executive-Summary)
  - [Highlevel Metrics](#highlevel-metrics)
  - [Projects Compatibility](#projects-compatibility)
  - [Package Compatibility](#package-compatibility)
  - [API Compatibility](#api-compatibility)
  - [Binding Redirect Configuration](#binding-redirect-configuration)
- [Aggregate NuGet packages details](#aggregate-nuget-packages-details)
- [Top API Migration Challenges](#top-api-migration-challenges)
  - [Technologies and Features](#technologies-and-features)
  - [Most Frequent API Issues](#most-frequent-api-issues)
- [Projects Relationship Graph](#projects-relationship-graph)
- [Project Details](#project-details)

  - [DynamicFirewallUpdater\DynamicFirewallUpdater.csproj](#dynamicfirewallupdaterdynamicfirewallupdatercsproj)


## Executive Summary

### Highlevel Metrics

| Metric | Count | Status |
| :--- | :---: | :--- |
| Total Projects | 1 | All require upgrade |
| Total NuGet Packages | 17 | All compatible |
| Total Code Files | 8 |  |
| Total Code Files with Incidents | 2 |  |
| Total Lines of Code | 436 |  |
| Total Number of Issues | 2 |  |
| Estimated LOC to modify | 1+ | at least 0.2% of codebase |

### Projects Compatibility

| Project | Target Framework | Difficulty | Package Issues | API Issues | Binding Issues | Est. LOC Impact | Description |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: | :--- |
| [DynamicFirewallUpdater\DynamicFirewallUpdater.csproj](#dynamicfirewallupdaterdynamicfirewallupdatercsproj) | net9.0 | 🟢 Low | 0 | 1 | 0 | 1+ | DotNetCoreApp, Sdk Style = True |

### Package Compatibility

| Status | Count | Percentage |
| :--- | :---: | :---: |
| ✅ Compatible | 17 | 100.0% |
| ⚠️ Incompatible | 0 | 0.0% |
| 🔄 Upgrade Recommended | 0 | 0.0% |
| ***Total NuGet Packages*** | ***17*** | ***100%*** |

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 1 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 420 |  |
| ***Total APIs Analyzed*** | ***421*** |  |

## Aggregate NuGet packages details

| Package | Current Version | Suggested Version | Projects | Description |
| :--- | :---: | :---: | :--- | :--- |
| Azure.Identity | 1.21.0 |  | [DynamicFirewallUpdater.csproj](#dynamicfirewallupdaterdynamicfirewallupdatercsproj) | ✅Compatible |
| Azure.ResourceManager | 1.14.0 |  | [DynamicFirewallUpdater.csproj](#dynamicfirewallupdaterdynamicfirewallupdatercsproj) | ✅Compatible |
| Azure.ResourceManager.Sql | 1.4.0 |  | [DynamicFirewallUpdater.csproj](#dynamicfirewallupdaterdynamicfirewallupdatercsproj) | ✅Compatible |
| Flurl.Http | 4.0.2 |  | [DynamicFirewallUpdater.csproj](#dynamicfirewallupdaterdynamicfirewallupdatercsproj) | ✅Compatible |
| IPNetwork2 | 4.3.0 |  | [DynamicFirewallUpdater.csproj](#dynamicfirewallupdaterdynamicfirewallupdatercsproj) | ✅Compatible |
| Microsoft.Extensions.Configuration | 10.0.10 |  | [DynamicFirewallUpdater.csproj](#dynamicfirewallupdaterdynamicfirewallupdatercsproj) | ✅Compatible |
| Microsoft.Extensions.Configuration.Binder | 10.0.10 |  | [DynamicFirewallUpdater.csproj](#dynamicfirewallupdaterdynamicfirewallupdatercsproj) | ✅Compatible |
| Microsoft.Extensions.Configuration.CommandLine | 10.0.10 |  | [DynamicFirewallUpdater.csproj](#dynamicfirewallupdaterdynamicfirewallupdatercsproj) | ✅Compatible |
| Microsoft.Extensions.Configuration.EnvironmentVariables | 10.0.10 |  | [DynamicFirewallUpdater.csproj](#dynamicfirewallupdaterdynamicfirewallupdatercsproj) | ✅Compatible |
| Microsoft.Extensions.Configuration.Json | 10.0.10 |  | [DynamicFirewallUpdater.csproj](#dynamicfirewallupdaterdynamicfirewallupdatercsproj) | ✅Compatible |
| Microsoft.Extensions.Configuration.UserSecrets | 10.0.10 |  | [DynamicFirewallUpdater.csproj](#dynamicfirewallupdaterdynamicfirewallupdatercsproj) | ✅Compatible |
| RecursiveDataAnnotationsValidation | 2.2.0 |  | [DynamicFirewallUpdater.csproj](#dynamicfirewallupdaterdynamicfirewallupdatercsproj) | ✅Compatible |
| Serilog | 4.4.0 |  | [DynamicFirewallUpdater.csproj](#dynamicfirewallupdaterdynamicfirewallupdatercsproj) | ✅Compatible |
| Serilog.Settings.Configuration | 10.0.1 |  | [DynamicFirewallUpdater.csproj](#dynamicfirewallupdaterdynamicfirewallupdatercsproj) | ✅Compatible |
| Serilog.Sinks.Console | 6.1.1 |  | [DynamicFirewallUpdater.csproj](#dynamicfirewallupdaterdynamicfirewallupdatercsproj) | ✅Compatible |
| Serilog.Sinks.Debug | 3.0.0 |  | [DynamicFirewallUpdater.csproj](#dynamicfirewallupdaterdynamicfirewallupdatercsproj) | ✅Compatible |
| Serilog.Sinks.File | 7.0.0 |  | [DynamicFirewallUpdater.csproj](#dynamicfirewallupdaterdynamicfirewallupdatercsproj) | ✅Compatible |

## Top API Migration Challenges

### Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |

### Most Frequent API Issues

| API | Count | Percentage | Category |
| :--- | :---: | :---: | :--- |
| M:Microsoft.Extensions.Configuration.ConfigurationBinder.Get''1(Microsoft.Extensions.Configuration.IConfiguration) | 1 | 100.0% | Binary Incompatible |

## Projects Relationship Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart LR
    P1["<b>📦&nbsp;DynamicFirewallUpdater.csproj</b><br/><small>net9.0</small>"]
    click P1 "#dynamicfirewallupdaterdynamicfirewallupdatercsproj"

```

## Project Details

<a id="dynamicfirewallupdaterdynamicfirewallupdatercsproj"></a>
### DynamicFirewallUpdater\DynamicFirewallUpdater.csproj

#### Project Info

- **Current Target Framework:** net9.0
- **Proposed Target Framework:** net10.0
- **SDK-style**: True
- **Project Kind:** DotNetCoreApp
- **Dependencies**: 0
- **Dependants**: 0
- **Number of Files**: 8
- **Number of Files with Incidents**: 2
- **Lines of Code**: 436
- **Estimated LOC to modify**: 1+ (at least 0.2% of the project)

#### Dependency Graph

Legend:
📦 SDK-style project
⚙️ Classic project

```mermaid
flowchart TB
    subgraph current["DynamicFirewallUpdater.csproj"]
        MAIN["<b>📦&nbsp;DynamicFirewallUpdater.csproj</b><br/><small>net9.0</small>"]
        click MAIN "#dynamicfirewallupdaterdynamicfirewallupdatercsproj"
    end

```

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 1 | High - Require code changes |
| 🟡 Source Incompatible | 0 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 420 |  |
| ***Total APIs Analyzed*** | ***421*** |  |

