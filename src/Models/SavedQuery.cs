using XperienceCommunity.SqlBrowser.Admin.Generated;

namespace XperienceCommunity.SqlBrowser.Models;

/// <summary>
/// Model which maps info from <see cref="SqlBrowserSavedQueryInfo"/> to a client-side object.
/// </summary>
public class SavedQuery(SqlBrowserSavedQueryInfo source)
{
    public int ID { get; set; } = source.SqlBrowserSavedQueryId;


    public string? Name { get; set; } = source.SqlBrowserSavedQueryName;


    public string? Text { get; set; } = source.SqlBrowserSavedQueryText;
}
