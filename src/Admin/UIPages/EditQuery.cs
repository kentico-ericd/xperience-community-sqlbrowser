using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;

using Kentico.Xperience.Admin.Base;

using System.Data;

using XperienceCommunity.SqlBrowser.Admin.UIPages.Properties;
using XperienceCommunity.SqlBrowser.Services;
using XperienceCommunity.SqlBrowser.Models;
using XperienceCommunity.SqlBrowser.Admin.Generated;

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
    IPageLinkGenerator pageLinkGenerator,
    IInfoProvider<SqlBrowserSavedQueryInfo> savedQueryProvider) : Page<EditSqlTemplateClientProperties>
{
    public override Task<EditSqlTemplateClientProperties> ConfigureTemplateProperties(EditSqlTemplateClientProperties properties)
    {
        properties.Tables =
            cache.Load(LoadTables, new CacheSettings(10, $"{nameof(EditQuery)}|{nameof(ConfigureTemplateProperties)}"));
        properties.Query = sqlBrowserResultProvider.GetQuery();
        properties.SavedQueries = savedQueryProvider.Get()
            .GetEnumerableTypedResult()
            .Select(q => new SavedQuery(q));

        return Task.FromResult(properties);
    }


    [PageCommand(CommandName = nameof(Notify))]
    public Task<ICommandResponse> Notify(string message) =>
        Task.FromResult(Response().AddSuccessMessage(message));


    [PageCommand(CommandName = nameof(RunSql))]
    public Task<ICommandResponse> RunSql(string query)
    {
        sqlBrowserResultProvider.SetQuery(query);
        string navigationUrl = pageLinkGenerator.GetPath<ResultListing>();

        return Task.FromResult((ICommandResponse)NavigateTo(navigationUrl));
    }


    [PageCommand(CommandName = nameof(DeleteQuery))]
    public Task<ICommandResponse> DeleteQuery(int id)
    {
        var query = savedQueryProvider.Get()
            .TopN(1)
            .WhereEquals(nameof(SqlBrowserSavedQueryInfo.SqlBrowserSavedQueryId), id)
            .FirstOrDefault();
        if (query is null)
        {
            return Task.FromResult(Response().AddErrorMessage($"Query {id} not found"));
        }

        query.Delete();

        return Task.FromResult(Response().AddSuccessMessage("Query deleted! Reload the page to view the updated list"));
    }


    [PageCommand(CommandName = nameof(SaveQuery))]
    public Task<ICommandResponse> SaveQuery(string[] parameters)
    {
        if (parameters.Length < 2)
        {
            return Task.FromResult(Response().AddErrorMessage("Received incorrect number of parameters"));
        }

        string name = parameters[0];
        string text = parameters[1];
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(text))
        {
            return Task.FromResult(Response().AddErrorMessage("Received empty parameter"));
        }

        try
        {
            new SqlBrowserSavedQueryInfo
            {
                SqlBrowserSavedQueryName = name,
                SqlBrowserSavedQueryText = text
            }.Insert();
        }
        catch (Exception ex)
        {
            return Task.FromResult(Response().AddErrorMessage(ex.Message));
        }

        return Task.FromResult(Response().AddSuccessMessage("Query saved! Reload the page to view the updated list"));
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
