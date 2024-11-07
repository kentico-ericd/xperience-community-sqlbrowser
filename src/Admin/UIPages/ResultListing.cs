using CMS.Base;
using CMS.Core;
using CMS.Helpers;

using Kentico.Xperience.Admin.Base;

using XperienceCommunity.SqlBrowser.Enum;
using XperienceCommunity.SqlBrowser.Services;

namespace XperienceCommunity.SqlBrowser.Admin.UIPages;

/// <summary>
/// Listing UI page which displays the results of a SQL query.
/// </summary>
[UINavigation(false)]
public class ResultListing(
    ISqlBrowserResultProvider sqlBrowserQueryProvider,
    ISqlBrowserExporter sqlBrowserExporter,
    IEventLogService eventLogService) : DataContainerListingPage
{
    public override Task ConfigurePage()
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
            PageConfiguration.HeaderActions.AddCommand("Export to CSV", nameof(ExportToCsv));
            PageConfiguration.HeaderActions.AddCommand("Export to Excel", nameof(ExportToXls));
            PageConfiguration.HeaderActions.AddCommand("Export to JSON", nameof(ExportToJson));
            PageConfiguration.AddEditRowAction<ViewRecord>();
        }

        return base.ConfigurePage();
    }


    [PageCommand]
    public Task<ICommandResponse> ExportToCsv() => Export(SqlBrowserExportType.Csv);


    [PageCommand]
    public Task<ICommandResponse> ExportToXls() => Export(SqlBrowserExportType.Excel);


    [PageCommand]
    public Task<ICommandResponse> ExportToJson() => Export(SqlBrowserExportType.Json);


    [PageCommand]
    public Task<ICommandResponse<RowActionResult>> ViewRecord(int id) =>
        Task.FromResult(ResponseFrom(new RowActionResult(false)).AddSuccessMessage(sqlBrowserQueryProvider.GetRowAsText(id)));


    protected override object GetIdentifier(IDataContainer dataContainer) =>
        ValidationHelper.GetInteger(dataContainer[SqlBrowserResultProvider.ROW_IDENTIFIER_COLUMN], -1);


    protected override Task<IEnumerable<IDataContainer>> LoadDataContainers(CancellationToken cancellationToken) =>
        Task.FromResult(sqlBrowserQueryProvider.GetRowsAsDataContainer());


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


    private Task<ICommandResponse> Export(SqlBrowserExportType exportType)
    {
        string? exportedPath = null;
        try
        {
            switch (exportType)
            {
                case SqlBrowserExportType.Csv:
                    exportedPath = sqlBrowserExporter.ExportToCsv();
                    break;
                case SqlBrowserExportType.Excel:
                    exportedPath = sqlBrowserExporter.ExportToXls();
                    break;
                case SqlBrowserExportType.Json:
                    exportedPath = sqlBrowserExporter.ExportToJson();
                    break;
                default:
                    break;
            }
        }
        catch (Exception ex)
        {
            eventLogService.LogException(nameof(ResultListing), nameof(Export), ex);
        }

        if (!string.IsNullOrEmpty(exportedPath))
        {
            return Task.FromResult(Response().AddSuccessMessage($"Exported results to {exportedPath}"));
        }
        else
        {
            return Task.FromResult(Response().AddErrorMessage("Export failed, please check the Event log for errors"));
        }
    }
}
