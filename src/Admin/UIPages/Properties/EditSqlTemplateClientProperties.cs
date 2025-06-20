using Kentico.Xperience.Admin.Base;

using XperienceCommunity.SqlBrowser.Models;

namespace XperienceCommunity.SqlBrowser.Admin.UIPages.Properties;

/// <summary>
/// Properties used by the client template for editing SQL queries.
/// </summary>
public class EditSqlTemplateClientProperties : TemplateClientProperties
{
    /// <summary>
    /// The list of tables and columns present in the database.
    /// </summary>
    public IEnumerable<DatabaseTable> Tables { get; set; } = [];


    /// <summary>
    /// The query to pre-fill in the editor text area.
    /// </summary>
    public string? Query { get; set; }
}
