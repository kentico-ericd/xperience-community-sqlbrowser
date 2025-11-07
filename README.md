# Xperience Community: SQL Browser

[![Nuget](https://img.shields.io/nuget/v/XperienceCommunity.SqlBrowser)](https://www.nuget.org/packages/XperienceCommunity.SqlBrowser#versions-body-tab)
[![build](https://github.com/kentico-ericd/xperience-community-sqlbrowser/actions/workflows/build.yml/badge.svg)](https://github.com/kentico-ericd/xperience-community-sqlbrowser/actions/workflows/build.yml)

## Description

This new module found in the **Development** category allows users to execute SQL queries within the administration UI and view the results in a table. SQL results can be exported to common file types, and result rows can be clicked for a detailed view.

## Library Version Matrix

| Xperience Version | Library Version |
| ----------------- | --------------- |
| >= 29.0.0         | < 3.0.0         |
| >= 30.6.0         | >= 3.0.1        |
| >= 30.11.2        | >= 4.0.0        |

## :gear: Package Installation

Add the package to your application using the .NET CLI

```powershell
dotnet add package XperienceCommunity.SqlBrowser
```

## 🚀 Quick Start

Add the following to your application's startup code:

```cs
builder.Services.AddSqlBrowser();
```

## Full Instructions

View the [Usage Guide](docs/Usage-Guide.md) for more detailed instructions.
