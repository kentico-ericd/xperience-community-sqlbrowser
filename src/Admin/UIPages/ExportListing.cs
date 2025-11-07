using CMS.Base;
using CMS.Helpers;
using CMS.Membership;

using Kentico.Xperience.Admin.Base;

using XperienceCommunity.SqlBrowser.Admin.UIPages.Properties;
using XperienceCommunity.SqlBrowser.Services;

namespace XperienceCommunity.SqlBrowser.Admin.UIPages;

[UIEvaluatePermission(SystemPermissions.VIEW)]
public class ExportListing(ISqlBrowserExporter sqlBrowserExporter) : DataContainerListingPage
{
    private const string ROW_IDENTIFIER_COLUMN = "export_filename";


    public override async Task ConfigurePage()
    {
        PageConfiguration.ColumnConfigurations.AddColumn(ROW_IDENTIFIER_COLUMN, "Name");
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

        await base.ConfigurePage();
    }


    [PageCommand]
    public async Task<ICommandResponse<string>> GetBase64String(string fileName)
    {
        string exportDirectory = sqlBrowserExporter.GetExportDirectory();
        string fullPath = Path.Combine(exportDirectory, fileName);
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
            rows.Add(new DataContainer
            {
                [ROW_IDENTIFIER_COLUMN] = Path.GetFileName(file)
            });
        }

        return Task.FromResult(rows.AsEnumerable());
    }
}
