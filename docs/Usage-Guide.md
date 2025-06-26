# Usage Guide

## Configuration Options

You can customize the SQL browser behavior during application startup:

```cs
builder.Services.AddSqlBrowser(options =>
{
    options.UseSafeQuerySelect = false;
});
```

The following options are currently available:

| Property name                       | Default value | Description                                         |
| ----------------------------------- | ------------- | --------------------------------------------------- |
| [UseSafeQuerySelect](#sql-security) | `true`        | Enable/disable SQL validation for enhanced security |

### SQL Security

When `UseSafeQuerySelect` is enabled, the following SQL statements are blocked:

- **Data Modification**: INSERT, UPDATE, DELETE
- **Schema Operations**: CREATE, ALTER, DROP operations
- **Execution Statements**: EXECUTE statements
- **Dangerous Functions**: OPENROWSET, XP_CMDSHELL, etc.

The validator uses Microsoft.SqlServer.TransactSql.ScriptDom for proper SQL parsing, ensuring it correctly identifies statement types and won't be fooled by SQL injection attempts or obfuscated queries.

## Executing Queries

Open the new **SQL browser** application in the **Development** category. All database tables are listed on the right side for reference, and you can click a table to automatically generate a SELECT query for that table.

Enter your SQL query and click "Run."

![Query](/images/editquery.png)

The results of your query will be displayed in a table. Click on an individual row to view the full detail of that row's columns. The result set can be exported using the buttons above the table.

![Results](/images/results.png)

## Saving queries

If you have a query you'd like to run often, or very long and complex queries, you can save them for later use. Saved queries are shared for _all users_!

First, enter your query in the text box then click **Save**. You will be prompted to enter a name for the query- this is in the identifier format, so it will not accept spaces and some special characters. Once confirmed, you will see a new box below the editor which lists your saved queries.

To run a saved query, click the "Copy" icon (the paper) and the query will be pasted into the editor. You can then click **Run** to execute it, or modify it if needed. Queries can also be re-ordered by dragging, and deleted using the "Trash" icon.
