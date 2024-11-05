using Microsoft.Extensions.DependencyInjection;

using XperienceCommunity.SqlBrowser.Admin;
using XperienceCommunity.SqlBrowser.Services;

namespace XperienceCommunity.SqlBrowser;

/// <summary>
/// Contains methods to initialize the module during application startup.
/// </summary>
public static class StartupExtensions
{
    /// <summary>
    /// Registers services required by the module.
    /// </summary>
    public static IServiceCollection AddSqlBrowser(this IServiceCollection services)
    {
        services.AddSingleton<SqlBrowserModuleInstaller>();
        services.AddSingleton<ISqlBrowserExporter, SqlBrowserExporter>();
        services.AddSingleton<ISqlBrowserResultProvider, SqlBrowserResultProvider>();

        return services;
    }
}
