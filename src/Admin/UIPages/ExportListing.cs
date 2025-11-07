using CMS.Base;
using CMS.Helpers;
using CMS.Membership;

using Kentico.Xperience.Admin.Base;

using XperienceCommunity.SqlBrowser.Admin.UIPages.Properties;
using XperienceCommunity.SqlBrowser.Services;

namespace XperienceCommunity.SqlBrowser.Admin.UIPages;

/// <summary>
/// Listing page which displays all files in the export folder.
/// </summary>
[UINavigation(true)]
[UIEvaluatePermission(SystemPermissions.VIEW)]
public class ExportListing(ISqlBrowserExporter sqlBrowserExporter) : DataContainerListingPage
{
    private const string FILESIZE_COLUMN = "filesize";
    private const string ROW_IDENTIFIER_COLUMN = "export_filename";


    public override async Task ConfigurePage()
    {
        PageConfiguration.ColumnConfigurations
            .AddComponentColumn(
                "download",
                "@xperience-community/sql-browser/DownloadExport",
                "Download",
                modelRetriever: (formattedColumnValue, rowData) =>
                    new DownloadExportClientProperties
                    {
                        FileName = GetIdentifier(rowData).ToString(),
                    },
                loadedExternally: true,
                sortable: false);
        PageConfiguration.ColumnConfigurations.AddColumn(ROW_IDENTIFIER_COLUMN, "Name");
        PageConfiguration.ColumnConfigurations.AddColumn(FILESIZE_COLUMN, "Size (KB)", minWidth: 120);
        PageConfiguration.TableActions.AddDeleteAction(nameof(DeleteExport));

        await base.ConfigurePage();
    }


    /// <summary>
    /// Deletes an export file.
    /// </summary>
    [PageCommand]
    public Task<ICommandResponse<RowActionResult>> DeleteExport(string fileName)
    {
        string exportDirectory = sqlBrowserExporter.GetExportDirectory();
        string fullPath = Path.Combine(exportDirectory, fileName);
        if (!File.Exists(fullPath))
        {
            return Task.FromResult(ResponseFrom(new RowActionResult(false)).AddErrorMessage("File not found!"));
        }

        File.Delete(fullPath);

        return Task.FromResult(ResponseFrom(new RowActionResult(true)).AddSuccessMessage("File deleted."));
    }


    /// <summary>
    /// Gets the Base64 data of an export file.
    /// </summary>
    [PageCommand]
    public async Task<ICommandResponse<string>> GetBase64String(string fileName)
    {
        string exportDirectory = sqlBrowserExporter.GetExportDirectory();
        string fullPath = Path.Combine(exportDirectory, fileName);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("Export file not found.", fileName);
        }

        byte[] bytes = await File.ReadAllBytesAsync(fullPath);

        return ResponseFrom(Convert.ToBase64String(bytes));
    }


    protected override object GetIdentifier(IDataContainer dataContainer) =>
        ValidationHelper.GetString(dataContainer[ROW_IDENTIFIER_COLUMN], string.Empty);


    protected override Task<IEnumerable<IDataContainer>> LoadDataContainers(CancellationToken cancellationToken)
    {
        var rows = new List<IDataContainer>();
        string exportDirectory = sqlBrowserExporter.GetExportDirectory();
        if (!Directory.Exists(exportDirectory))
        {
            return Task.FromResult(rows.AsEnumerable());
        }

        string[] files = Directory.GetFiles(exportDirectory);
        foreach (string file in files)
        {
            var fileInfo = new FileInfo(file);
            double fileSizeInKB = (double)fileInfo.Length / 1024;
            rows.Add(new DataContainer
            {
                [ROW_IDENTIFIER_COLUMN] = Path.GetFileName(file),
                [FILESIZE_COLUMN] = Math.Round(fileSizeInKB, 2)
            });
        }

        return Task.FromResult(rows.AsEnumerable());
    }
}
