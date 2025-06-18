using System.Data;
using System.Dynamic;
using System.Text;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;

using XperienceCommunity.SqlBrowser.Helpers;

namespace XperienceCommunity.SqlBrowser.Services;

/// <summary>
/// Default implementation of <see cref="ISqlBrowserResultProvider"/>.
/// </summary>
public class SqlBrowserResultProvider(IEventLogService eventLogService) : ISqlBrowserResultProvider
{
    private string? query;
    private DataSet? result;
    public const string ROW_IDENTIFIER_COLUMN = $"{nameof(SqlBrowserResultProvider)}_result_identifier";
    public static bool SafeQuerySelect { get; set; } = true;

    public IEnumerable<string> GetColumnNames()
    {
        EnsureResult();

        if (ResultsAreEmpty())
        {
            return [];
        }

        return result!.Tables[0].Columns.OfType<DataColumn>().Select(c => c.ColumnName);
    }


    public Task<string> GetRowAsText(int rowIdentifier)
    {
        EnsureResult();

        var row = result!.Tables[0].Rows[rowIdentifier] ?? throw new InvalidOperationException($"Failed to load row {rowIdentifier}");
        var textResult = new StringBuilder();
        foreach (string col in GetColumnNames())
        {
            textResult
                .Append(col)
                .Append(": ")
                .Append(row[col])
                .Append(Environment.NewLine);
        }

        return Task.FromResult(textResult.ToString());
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


    public Task<IEnumerable<IDataContainer>> GetRowsAsDataContainer()
    {
        EnsureResult();

        if (ResultsAreEmpty())
        {
            return Task.FromResult<IEnumerable<IDataContainer>>([]);
        }

        var columnNames = GetColumnNames();
        var containers = result!.Tables[0].Rows.OfType<DataRow>().Select((row, i) =>
        {
            var data = new DataContainer
            {
                [ROW_IDENTIFIER_COLUMN] = i,
            };
            foreach (string col in columnNames)
            {
                data[col] = row[col];
            }

            return data;
        });

        return Task.FromResult<IEnumerable<IDataContainer>>(containers);
    }


    public Task<IEnumerable<dynamic>> GetRowsAsDynamic()
    {
        EnsureResult();

        if (ResultsAreEmpty())
        {
            return Task.FromResult<IEnumerable<dynamic>>([]);
        }

        var columnNames = GetColumnNames();
        var dynamics = result!.Tables[0].Rows.OfType<DataRow>().Select(row =>
        {
            var obj = new ExpandoObject();
            foreach (string col in columnNames)
            {
                (obj as IDictionary<string, object>).Add(col, row[col]);
            }

            return obj;
        });

        return Task.FromResult<IEnumerable<dynamic>>(dynamics);
    }


    public string? GetQuery() => query;


    public void SetQuery(string queryText)
    {
        result = null;
        query = queryText;
    }


    /// <summary>
    /// Ensures <see cref="result"/> is not null and if so, sets it by executing the underlying query.
    /// </summary>
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
            if (SafeQuerySelect)
            {
                var validator = new SqlStatementValidator();
                var validationResult = validator.ValidateSqlStatement(query);

                if (!validationResult.IsValid)
                {
                    eventLogService.LogWarning(nameof(SqlBrowserResultProvider), nameof(EnsureResult),
                       $"User attempted to execute invalid query:{Environment.NewLine}{query}{Environment.NewLine}Error: {validationResult.ErrorMessage}");
                    throw new InvalidOperationException($"Invalid SQL query: {validationResult.ErrorMessage}");
                }
            }

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


    /// <summary>
    /// Returns true if <see cref="result"/> is null or contains no tables.
    /// </summary>
    private bool ResultsAreEmpty() => result is null || result.Tables.Count == 0;
}
