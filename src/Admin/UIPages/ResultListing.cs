using CMS.Base;
using CMS.Helpers;
using CMS.Membership;

using Kentico.Xperience.Admin.Base;

using XperienceCommunity.SqlBrowser.Services;

namespace XperienceCommunity.SqlBrowser.Admin.UIPages;

/// <summary>
/// Listing UI page which displays the results of a SQL query.
/// </summary>
[UINavigation(false)]
[UIEvaluatePermission(SystemPermissions.VIEW)]
public class ResultListing(
    ISqlBrowserResultProvider sqlBrowserQueryProvider,
    IUIPermissionEvaluator permissionEvaluator) : DataContainerListingPage
{
    public override async Task ConfigurePage()
    {
        int recordCount = sqlBrowserQueryProvider.GetTotalRecordCount();
        if (recordCount == 0)
        {
            PageConfiguration.Callouts = [
                new CalloutConfiguration
                {
                    Headline = "No results",
                    Content = "Query result has no data, please check the Event log for errors or modify your query",
                    Placement = CalloutPlacement.OnPaper,
                    Type = CalloutType.FriendlyWarning
                }];
        }
        else
        {
            ConfigureColumns();
            PageConfiguration.Caption = $"Results ({recordCount})";
            PageConfiguration.AddEditRowAction<ViewRecord>();

            var exportPermission = await permissionEvaluator.Evaluate(SqlBrowserApplicationPage.EXPORT_PERMISSION);
            if (exportPermission.Succeeded)
            {
                PageConfiguration.EnableExport(new ListingExportConfiguration
                {
                    FileName = $"SqlBrowser_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                });
            }
        }

        await base.ConfigurePage();
    }


    protected override object GetIdentifier(IDataContainer dataContainer) =>
        ValidationHelper.GetInteger(dataContainer[SqlBrowserResultProvider.ROW_IDENTIFIER_COLUMN], -1);


    protected override Task<IEnumerable<IDataContainer>> LoadDataContainers(CancellationToken cancellationToken) =>
        sqlBrowserQueryProvider.GetRowsAsDataContainer();


    private void ConfigureColumns()
    {
        var columnNames = sqlBrowserQueryProvider.GetColumnNames();
        // Get largest header length to set all column min width- avoids text jumbling
        int columnMinWidth = Math.Ceiling(columnNames.Max(col => col.Length) * 0.7).ToInteger();
        foreach (string col in columnNames)
        {
            PageConfiguration.ColumnConfigurations.AddColumn(col, col, minWidth: columnMinWidth);
        }
    }
}
