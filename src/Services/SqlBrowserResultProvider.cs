using System.Data;
using System.Dynamic;
using System.Text;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;

using XperienceCommunity.SqlBrowser.Admin.UIPages;
using XperienceCommunity.SqlBrowser.Infos;

namespace XperienceCommunity.SqlBrowser.Services;
public class SqlBrowserResultProvider : ISqlBrowserResultProvider
{
    private DataSet? result;
    private SqlBrowserQueryInfo? query;
    private readonly IEventLogService eventLogService;


    public const string ROW_IDENTIFIER_COLUMN = $"{nameof(ResultListing)}_result_identifier";


    public SqlBrowserResultProvider(IEventLogService eventLogService) => this.eventLogService = eventLogService;


    public IEnumerable<string> GetColumnNames()
    {
        EnsureResult();

        return result?.Tables.Count > 0 ?
            result.Tables[0].Columns.OfType<DataColumn>().Select(c => c.ColumnName) : [];
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


    public IEnumerable<IDataContainer>? GetRowsAsDataContainer()
    {
        EnsureResult();

        if (result?.Tables.Count == 0)
        {
            return null;
        }

        return result?.Tables[0].Rows.OfType<DataRow>().Select((row, i) =>
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


    public IEnumerable<dynamic>? GetRowsAsDynamic()
    {
        EnsureResult();

        if (result?.Tables.Count == 0)
        {
            return null;
        }

        return result?.Tables[0].Rows.OfType<DataRow>().Select((row, i) =>
        {
            var obj = new ExpandoObject();
            foreach (string col in GetColumnNames())
            {
                (obj as IDictionary<string, object>).Add(col, row[col]);
            }

            return obj;
        });
    }


    public void SetQuery(SqlBrowserQueryInfo queryInfo)
    {
        result = null;
        query = queryInfo;
    }


    private void EnsureResult()
    {
        if (result is not null)
        {
            return;
        }

        if (query is null || string.IsNullOrEmpty(query.SqlBrowserQueryText))
        {
            throw new InvalidOperationException($"No query present in {nameof(SqlBrowserResultProvider)}");
        }

        try
        {
            result = ConnectionHelper.ExecuteQuery(query.SqlBrowserQueryText, null, QueryTypeEnum.SQLQuery);
        }
        catch (Exception ex)
        {
            result = new DataSet();
            eventLogService.LogException(nameof(SqlBrowserResultProvider), nameof(EnsureResult), ex);
        }
    }
}
