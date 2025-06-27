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
    public override async Task<EditSqlTemplateClientProperties> ConfigureTemplateProperties(EditSqlTemplateClientProperties properties)
    {
        properties.Tables =
            cache.Load(LoadTables, new CacheSettings(10, $"{nameof(EditQuery)}|{nameof(ConfigureTemplateProperties)}"));
        properties.Query = sqlBrowserResultProvider.GetQuery();
        properties.SavedQueries = (await GetSavedQueries()).Select(q => new SavedQuery(q));

        return properties;
    }


    [PageCommand]
    public Task<ICommandResponse> Notify(string message) =>
        Task.FromResult(Response().AddSuccessMessage(message));


    [PageCommand]
    public Task<ICommandResponse> RunSql(string query)
    {
        sqlBrowserResultProvider.SetQuery(query);
        string navigationUrl = pageLinkGenerator.GetPath<ResultListing>();

        return Task.FromResult((ICommandResponse)NavigateTo(navigationUrl));
    }


    [PageCommand]
    public Task<ICommandResponse<int>> DeleteQuery(int id)
    {
        var query = savedQueryProvider.Get()
            .TopN(1)
            .WhereEquals(nameof(SqlBrowserSavedQueryInfo.SqlBrowserSavedQueryId), id)
            .FirstOrDefault();
        if (query is null)
        {
            return Task.FromResult(ResponseFrom(0).AddErrorMessage($"Query {id} not found"));
        }

        query.Delete();

        return Task.FromResult(ResponseFrom(id).AddSuccessMessage("Query deleted!"));
    }


    [PageCommand]
    public Task<ICommandResponse<SavedQuery?>> SaveQuery(SavedQuery query)
    {
        if (string.IsNullOrEmpty(query.Name) || string.IsNullOrEmpty(query.Text))
        {
            return Task.FromResult(ResponseFrom<SavedQuery?>(null).AddErrorMessage("Received empty parameter"));
        }

        int newOrder = 0;
        var queryOrders = savedQueryProvider.Get()
            .AsSingleColumn(nameof(SqlBrowserSavedQueryInfo.SqlBrowserSavedQueryOrder))
            .GetListResult<int>();
        if (queryOrders.Any())
        {
            newOrder = queryOrders.Max() + 1;
        }

        try
        {
            var newQuery = new SqlBrowserSavedQueryInfo
            {
                SqlBrowserSavedQueryName = query.Name,
                SqlBrowserSavedQueryText = query.Text,
                SqlBrowserSavedQueryOrder = newOrder
            };
            newQuery.Insert();
            query.Order = newOrder;
            query.ID = newQuery.SqlBrowserSavedQueryId;

            return Task.FromResult(ResponseFrom<SavedQuery?>(query).AddSuccessMessage("Query saved!"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(ResponseFrom<SavedQuery?>(null).AddErrorMessage(ex.Message));
        }
    }


    [PageCommand]
    public async Task<ICommandResponse<bool>> UpdateSavedOrder(SavedQuery[] newOrder)
    {
        var originalQueries = (await GetSavedQueries()).ToList();
        foreach (var newQuery in newOrder)
        {
            var original = originalQueries.Find(q => q.SqlBrowserSavedQueryId == newQuery.ID);
            if (original is null)
            {
                return ResponseFrom(false).AddErrorMessage($"Failed to update order: query {newQuery.ID} not found");
            }

            original.SqlBrowserSavedQueryOrder = newQuery.Order;
            original.Update();
        }

        return ResponseFrom(true);
    }


    private Task<IEnumerable<SqlBrowserSavedQueryInfo>> GetSavedQueries() =>
        savedQueryProvider.Get()
            .OrderBy(nameof(SqlBrowserSavedQueryInfo.SqlBrowserSavedQueryOrder))
            .GetEnumerableTypedResultAsync();


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
