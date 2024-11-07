using CMS.Base;

namespace XperienceCommunity.SqlBrowser.Services;

/// <summary>
/// Contains methods for retrieving results from the connected database.
/// </summary>
public interface ISqlBrowserResultProvider
{
    /// <summary>
    /// Gets all column names in the current result set.
    /// </summary>
    public IEnumerable<string> GetColumnNames();


    /// <summary>
    /// Gets a record from the current result set, transformed into a human-readable format.
    /// </summary>
    /// <param name="rowIdentifier">The identifier of the record to retrieve.</param>
    public string GetRowAsText(int rowIdentifier);


    /// <summary>
    /// Gets the number of records in the result set.
    /// </summary>
    public int GetTotalRecordCount();


    /// <summary>
    /// Gets the current result set converted to <see cref="IDataContainer"/>s. Automatically sets the identifier for each container to
    /// be retrieved later by <see cref="GetRowAsText"/>.
    /// </summary>
    public IEnumerable<IDataContainer> GetRowsAsDataContainer();


    /// <summary>
    /// Gets the current result set converted to dynamic objects. 
    /// </summary>
    public IEnumerable<dynamic> GetRowsAsDynamic();


    /// <summary>
    /// Sets the query to retrieve results from, and clears the current result set.
    /// </summary>
    public void SetQuery(string queryText);
}
