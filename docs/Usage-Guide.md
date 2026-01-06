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

Open the new **SQL browser** application in the **Development** category. Enter your SQL query and click "Run." A list of all tables in your database can be seen by clicking the **Tables** button in the editor. If you click a table name, a SELECT query will be generated for you.

![Query](/images/editquery.png)

The results of your query will be displayed in a table. Click on an individual row to view the full detail of that row's columns.

![Results](/images/results.png)

## Exporting results

After executing a query, click the **Export** button in the listing header to export the results to the server's ~/App_Data/Export folder. A dialog will appear allowing you to choose the export type (CSV, Excel, or JSON) and the file name. If the file name is empty, one will be automatically generated based on the current date and time:

![Export confirmation](/images/confirmation.png)

To view the contents of the export, click the **Exported data** navigation item in the side menu. Here, you can download individual export files to your local machine, or delete the files from the server.

## Saving queries

If you have a query you'd like to run often, or very long and complex queries, you can save them for later use. Saved queries are shared for _all users_!

First, enter your query in the text box then click **Save**. You will be prompted to enter a name for the query- this is in the identifier format, so it will not accept spaces and some special characters. Once confirmed, you will see a new box below the editor which lists your saved queries.

Saved queries can be re-ordered by dragging the items up and down the list. To run a query, click the "Execute" icon. There are some additional options in the menu, such as copying the query's text to the editor, and deleting the query:

![More options](/images/additionaloptions.png)
