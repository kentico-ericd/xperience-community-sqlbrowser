namespace XperienceCommunity.SqlBrowser.Models;

/// <summary>
/// Represents the options for the SQL browser module.
/// </summary>
public class SqlBrowserOptions
{
    /// <summary>
    /// If <c>true</c>, the SQL browser module may only execute SELECT statements. Default value is <c>true</c>.
    /// </summary>
    public bool UseSafeQuerySelect { get; set; } = true;
}
