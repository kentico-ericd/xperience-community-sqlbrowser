﻿# Xperience Community: SQL Browser

[![Nuget](https://img.shields.io/nuget/v/XperienceCommunity.SqlBrowser)](https://www.nuget.org/packages/XperienceCommunity.SqlBrowser#versions-body-tab)
[![build](https://github.com/kentico-ericd/xperience-community-sqlbrowser/actions/workflows/build.yml/badge.svg)](https://github.com/kentico-ericd/xperience-community-sqlbrowser/actions/workflows/build.yml)

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

Open the new __SQL browser__ application in the __Development__ category. All database tables and columns are listed below for reference. Enter your SQL query and click "Run."

![Query](/images/editquery.png)

The results of your query will be displayed in a table. Click on an individual row to view the full detail of that row's columns. The result set can be exported using the buttons above the table.

![Results](/images/results.png)
