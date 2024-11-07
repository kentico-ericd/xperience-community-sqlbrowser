using System.Data;
using System.Dynamic;
using System.Text;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;

namespace XperienceCommunity.SqlBrowser.Services;

/// <summary>
/// Default implementation of <see cref="ISqlBrowserResultProvider"/>.
/// </summary>
public class SqlBrowserResultProvider(IEventLogService eventLogService) : ISqlBrowserResultProvider
{
    private string? query;
    private DataSet? result;
    public const string ROW_IDENTIFIER_COLUMN = $"{nameof(SqlBrowserResultProvider)}_result_identifier";

    public IEnumerable<string> GetColumnNames()
    {
        EnsureResult();

        if (ResultsAreEmpty())
        {
            return [];
        }

        return result!.Tables[0].Columns.OfType<DataColumn>().Select(c => c.ColumnName);
    }


    public string GetRowAsText(int rowIdentifier)
    {
        EnsureResult();

        var row = (result?.Tables[0].Rows[rowIdentifier]) ?? throw new InvalidOperationException($"Failed to load row {rowIdentifier}");
        var textResult = new StringBuilder();
        foreach (string col in GetColumnNames())
        {
            textResult
                .Append(col)
                .Append(": ")
                .Append(row[col])
                .Append(Environment.NewLine);
        }

        return textResult.ToString();
    }


    public int GetTotalRecordCount()
    {
        EnsureResult();

        if (ResultsAreEmpty())
        {
            return 0;
        }

        return result!.Tables[0].Rows.Count;
    }


    public IEnumerable<IDataContainer> GetRowsAsDataContainer()
    {
        EnsureResult();

        if (ResultsAreEmpty())
        {
            return [];
        }

        return result!.Tables[0].Rows.OfType<DataRow>().Select((row, i) =>
        {
            var data = new DataContainer
            {
                [ROW_IDENTIFIER_COLUMN] = i,
            };
            foreach (string col in GetColumnNames())
            {
                data[col] = row[col];
            }

            return data;
        });
    }


    public IEnumerable<dynamic> GetRowsAsDynamic()
    {
        EnsureResult();

        if (ResultsAreEmpty())
        {
            return [];
        }

        var columnNames = GetColumnNames();
        return result!.Tables[0].Rows.OfType<DataRow>().Select((row, i) =>
        {
            var obj = new ExpandoObject();
            foreach (string col in columnNames)
            {
                (obj as IDictionary<string, object>).Add(col, row[col]);
            }

            return obj;
        });
    }


    public void SetQuery(string queryText)
    {
        result = null;
        query = queryText;
    }


    private void EnsureResult()
    {
        if (result is not null)
        {
            return;
        }

        if (string.IsNullOrEmpty(query))
        {
            throw new InvalidOperationException($"No query present in {nameof(SqlBrowserResultProvider)}");
        }

        try
        {
            result = ConnectionHelper.ExecuteQuery(query, null, QueryTypeEnum.SQLQuery);
            eventLogService.LogInformation(nameof(SqlBrowserResultProvider), nameof(EnsureResult),
                $"User executed query:{Environment.NewLine}{query}");
        }
        catch (Exception ex)
        {
            result = new DataSet();
            eventLogService.LogException(nameof(SqlBrowserResultProvider), nameof(EnsureResult), ex);
        }
    }


    private bool ResultsAreEmpty() => result is null || (result?.Tables.Count ?? 0) == 0;
}
