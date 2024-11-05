# Xperience Community: SQL Browser

[![Nuget](https://img.shields.io/nuget/v/XperienceCommunity.SqlBrowser)](https://www.nuget.org/packages/XperienceCommunity.SqlBrowser#versions-body-tab)
[![build](https://github.com/kentico-ericd/xperience-community-sqlbrowser/actions/workflows/build.yml/badge.svg)](https://github.com/kentico-ericd/xperience-community-sqlbrowser/actions/workflows/build.yml)

![Results](/images/results.png)

## Description

This new module found in the __Development__ category allows users to execute SQL queries within the administration UI and view the results in a table. SQL results can be exported to common file types, and result rows can be clicked for a detailed view.

## Library Version Matrix

| Xperience Version | Library Version |
| ----------------- | --------------- |
| 29.x.y            | >=1.x.y         |

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
