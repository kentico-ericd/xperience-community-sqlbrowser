using XperienceCommunity.SqlBrowser.Enum;

namespace XperienceCommunity.SqlBrowser.Services;

/// <summary>
/// Contains methods for exporting SQL query results to the filesystem.
/// </summary>
public interface ISqlBrowserExporter
{
    /// <summary>
    /// Exports the current SQL query results to the specified file type.
    /// </summary>
    /// <param name="exportType">The desired export file type.</param>
    /// <param name="fileName">The export file name, without extension. If <c>null</c>, the file name will be generated based on the
    /// current date and time.</param>
    /// <returns>The path of the resulting file.</returns>
    public Task<string> Export(SqlBrowserExportType exportType, string? fileName = null);


    /// <summary>
    /// Gets the full system path of the directory which stores exported SQL query results.
    /// </summary>
    public string GetExportDirectory();
}
