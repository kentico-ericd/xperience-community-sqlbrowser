using CMS.Base;

using XperienceCommunity.SqlBrowser.Infos;

namespace XperienceCommunity.SqlBrowser.Services;
public interface ISqlBrowserResultProvider
{
    public IEnumerable<string> GetColumnNames();


    public string GetRowAsText(int rowIdentifier);


    public IEnumerable<IDataContainer>? GetRowsAsDataContainer();


    public IEnumerable<dynamic>? GetRowsAsDynamic();


    public void SetQuery(SqlBrowserQueryInfo queryInfo);
}
