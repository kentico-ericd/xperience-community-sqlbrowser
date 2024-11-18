namespace XperienceCommunity.SqlBrowser.Services;

/// <summary>
/// Contains methods for exporting SQL query results to the filesystem.
/// </summary>
public interface ISqlBrowserExporter
{
    /// <summary>
    /// Exports the current SQL query results as a .csv file.
    /// </summary>
    /// <returns>The path of the resulting file.</returns>
    public Task<string> ExportToCsv();


    /// <summary>
    /// Exports the current SQL query results as a .xlsx file.
    /// </summary>
    /// <returns>The path of the resulting file.</returns>
    public Task<string> ExportToXls();


    /// <summary>
    /// Exports the current SQL query results as a .json file.
    /// </summary>
    /// <returns>The path of the resulting file.</returns>
    public Task<string> ExportToJson();
}
