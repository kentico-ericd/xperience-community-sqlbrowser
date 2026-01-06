using CMS.Base;
using CMS.Core;
using CMS.Helpers;
using CMS.Membership;

using Kentico.Xperience.Admin.Base;

using XperienceCommunity.SqlBrowser.Enum;
using XperienceCommunity.SqlBrowser.Models;
using XperienceCommunity.SqlBrowser.Services;

namespace XperienceCommunity.SqlBrowser.Admin.UIPages;

/// <summary>
/// Listing UI page which displays the results of a SQL query.
/// </summary>
[UINavigation(false)]
[UIEvaluatePermission(SystemPermissions.VIEW)]
public class ResultListing(
    ISqlBrowserResultProvider sqlBrowserQueryProvider,
    ISqlBrowserExporter sqlBrowserExporter,
    IEventLogService eventLogService,
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
            var exportPermission = await permissionEvaluator.Evaluate(SqlBrowserApplicationPage.EXPORT_PERMISSION);
            if (exportPermission.Succeeded)
            {
                PageConfiguration.HeaderActions.AddCommandWithConfirmation(
                    "Export",
                    nameof(Export),
                    "Choose export type",
                    "Export",
                    confirmationModel: typeof(ExportConfirmationDialogModel));
            }

            PageConfiguration.AddEditRowAction<ViewRecord>();
        }

        await base.ConfigurePage();
    }


    [PageCommand(Permission = SqlBrowserApplicationPage.EXPORT_PERMISSION)]
    public async Task<ICommandResponse> Export(ExportConfirmationDialogModel model)
    {
        string? exportedPath = null;
        var exportType = model.ExportType?.ToLower() switch
        {
            "csv" => SqlBrowserExportType.Csv,
            "excel" => SqlBrowserExportType.Excel,
            "json" => SqlBrowserExportType.Json,
            _ => SqlBrowserExportType.None
        };
        if (exportType == SqlBrowserExportType.None)
        {
            return Response().AddSuccessMessage($"Invalid export type.");
        }

        try
        {
            exportedPath = await sqlBrowserExporter.Export(exportType, model.FileName);
        }
        catch (Exception ex)
        {
            eventLogService.LogException(nameof(ResultListing), nameof(Export), ex);
        }

        if (!string.IsNullOrEmpty(exportedPath))
        {
            return Response().AddSuccessMessage($"Exported results to {exportedPath}");
        }
        else
        {
            return Response().AddErrorMessage("Export failed, please check the Event log for errors");
        }
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
