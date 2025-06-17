using Kentico.Xperience.Admin.Base;

using XperienceCommunity.SqlBrowser.Admin;

[assembly: CMS.RegisterModule(typeof(SqlBrowserModule))]
namespace XperienceCommunity.SqlBrowser.Admin;

/// <summary>
/// A module that initializes the SQL browser administration components.
/// </summary>
internal class SqlBrowserModule : AdminModule
{
    public SqlBrowserModule() : base(nameof(SqlBrowserModule))
    {
    }


    protected override void OnInit()
    {
        base.OnInit();

        RegisterClientModule("xperience-community", "sql-browser");
    }
}
