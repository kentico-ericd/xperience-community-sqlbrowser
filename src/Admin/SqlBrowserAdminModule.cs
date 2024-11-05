using CMS;
using CMS.Base;
using CMS.Core;

using Kentico.Xperience.Admin.Base;

using Microsoft.Extensions.DependencyInjection;

using XperienceCommunity.SqlBrowser.Admin;

[assembly: RegisterModule(typeof(SqlBrowserAdminModule))]

namespace XperienceCommunity.SqlBrowser.Admin;

/// <summary>
/// Manages administration features and integration.
/// </summary>
internal class SqlBrowserAdminModule : AdminModule
{
    private SqlBrowserModuleInstaller installer = null!;


    public SqlBrowserAdminModule() : base(nameof(SqlBrowserAdminModule)) { }


    protected override void OnInit(ModuleInitParameters parameters)
    {
        base.OnInit(parameters);

        RegisterClientModule("xperiencecommunity", "sqlbrowser");

        installer = parameters.Services.GetRequiredService<SqlBrowserModuleInstaller>();

        ApplicationEvents.PostStart.Execute += (_, _) => installer.Install();
    }
}
