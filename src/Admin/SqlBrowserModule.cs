using CMS.Base;
using CMS.Core;

using Kentico.Xperience.Admin.Base;

using Microsoft.Extensions.DependencyInjection;

using XperienceCommunity.SqlBrowser.Admin;

[assembly: CMS.RegisterModule(typeof(SqlBrowserModule))]
namespace XperienceCommunity.SqlBrowser.Admin;

/// <summary>
/// A module that initializes the SQL browser administration components.
/// </summary>
internal class SqlBrowserModule : AdminModule
{
    private SqlBrowserInstaller installer = null!;


    public SqlBrowserModule() : base(nameof(SqlBrowserModule))
    {
    }


    protected override void OnInit(ModuleInitParameters parameters)
    {
        base.OnInit(parameters);

        RegisterClientModule("xperience-community", "sql-browser");
        installer = parameters.Services.GetRequiredService<SqlBrowserInstaller>();
        ApplicationEvents.Initialized.Execute += (s, e) => installer.Install();
    }
}
