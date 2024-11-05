using Microsoft.Extensions.DependencyInjection;

using XperienceCommunity.SqlBrowser.Admin;
using XperienceCommunity.SqlBrowser.Services;

namespace XperienceCommunity.SqlBrowser;
public static class StartupExtensions
{
    public static IServiceCollection AddSqlBrowser(this IServiceCollection services)
    {
        services.AddSingleton<SqlBrowserModuleInstaller>();
        services.AddSingleton<ISqlBrowserExporter, SqlBrowserExporter>();
        services.AddSingleton<ISqlBrowserResultProvider, SqlBrowserResultProvider>();

        return services;
    }
}
