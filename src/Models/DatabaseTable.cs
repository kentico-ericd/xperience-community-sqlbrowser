namespace XperienceCommunity.SqlBrowser.Models;

/// <summary>
/// Represents a table's configuration in the database.
/// </summary>
public class DatabaseTable
{
    /// <summary>
    /// The table name.
    /// </summary>
    public string? Name { get; set; }


    /// <summary>
    /// The table columns.
    /// </summary>
    public IEnumerable<string> Columns { get; set; } = [];
}
