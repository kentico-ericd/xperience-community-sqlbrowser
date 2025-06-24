using XperienceCommunity.SqlBrowser.Admin.Generated;

namespace XperienceCommunity.SqlBrowser.Models;

/// <summary>
/// Model which maps info from <see cref="SqlBrowserSavedQueryInfo"/> to a client-side object.
/// </summary>
public class SavedQuery()
{
    public int ID { get; set; }


    public string? Name { get; set; }


    public string? Text { get; set; }


    public int Order { get; set; }


    public SavedQuery(SqlBrowserSavedQueryInfo source) : this()
    {
        ID = source.SqlBrowserSavedQueryId;
        Name = source.SqlBrowserSavedQueryName;
        Text = source.SqlBrowserSavedQueryText;
        Order = source.SqlBrowserSavedQueryOrder;
    }
}
