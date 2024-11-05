namespace XperienceCommunity.SqlBrowser.Services;
public interface ISqlBrowserExporter
{
    public Task<string> ExportToCsv();


    public Task<string> ExportToXls();
}
