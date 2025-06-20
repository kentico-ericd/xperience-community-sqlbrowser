using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;

using Kentico.Xperience.Admin.Base;

using System.Data;

using XperienceCommunity.SqlBrowser.Admin.UIPages.Properties;
using XperienceCommunity.SqlBrowser.Services;
using XperienceCommunity.SqlBrowser.Models;

namespace XperienceCommunity.SqlBrowser.Admin.UIPages;

/// <summary>
/// Edit UI page for submitting query text to <see cref="ISqlBrowserResultProvider"/>. 
/// </summary>
[UINavigation(false)]
[UIEvaluatePermission(SystemPermissions.VIEW)]
public class EditQuery(
    IEventLogService eventLogService,
    IProgressiveCache cache,
    ISqlBrowserResultProvider sqlBrowserResultProvider,
    IPageLinkGenerator pageLinkGenerator) : Page<EditSqlTemplateClientProperties>
{
    public override Task<EditSqlTemplateClientProperties> ConfigureTemplateProperties(EditSqlTemplateClientProperties properties)
    {
        var tables = cache.Load(LoadTables, new CacheSettings(10, $"{nameof(EditQuery)}|{nameof(ConfigureTemplateProperties)}"));
        if (!tables.Any())
        {
            throw new InvalidOperationException();
        }

        properties.Tables = tables;
        properties.Query = sqlBrowserResultProvider.GetQuery();

        return Task.FromResult(properties);
    }


    [PageCommand(CommandName = "RunSql")]
    public Task<ICommandResponse> RunSql(string query)
    {
        sqlBrowserResultProvider.SetQuery(query);
        string navigationUrl = pageLinkGenerator.GetPath<ResultListing>();

        return Task.FromResult((ICommandResponse)NavigateTo(navigationUrl));
    }


    private IEnumerable<DatabaseTable> LoadTables(CacheSettings cs)
    {
        try
        {
            string query = @"SELECT
                         T.name AS 'table',
                         C.name AS 'column'
                FROM     sys.objects AS T
                         JOIN sys.columns AS C ON T.object_id = C.object_id
                WHERE    T.type = 'U'
                ORDER BY T.name ASC";
            var result = ConnectionHelper.ExecuteQuery(query, null, QueryTypeEnum.SQLQuery);
            if (result.Tables.Count == 0)
            {
                cs.Cached = false;

                return [];
            }

            return result.Tables[0].Rows
                .OfType<DataRow>()
                .GroupBy(r => r["table"])
                .Select(group => new DatabaseTable
                {
                    Name = group.Key.ToString(),
                    Columns = group.Select(row => row["column"].ToString() ?? string.Empty)
                });
        }
        catch (Exception ex)
        {
            cs.Cached = false;
            eventLogService.LogException(nameof(EditQuery), nameof(LoadTables), ex);

            return [];
        }
    }
}
