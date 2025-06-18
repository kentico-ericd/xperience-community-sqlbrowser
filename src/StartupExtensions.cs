using Microsoft.Extensions.DependencyInjection;

using XperienceCommunity.SqlBrowser.Models;
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
    public static IServiceCollection AddSqlBrowser(this IServiceCollection services, Action<SqlBrowserOptions>? configureOptions = null)
    {
        if (configureOptions is not null)
        {
            var options = new SqlBrowserOptions();
            configureOptions(options);
            SqlBrowserResultProvider.SafeQuerySelect = options.UseSafeQuerySelect;
        }
        else
        {
            services.Configure<SqlBrowserOptions>(options => { });
        }

        services.AddSingleton<ISqlBrowserExporter, SqlBrowserExporter>();
        services.AddSingleton<ISqlBrowserResultProvider, SqlBrowserResultProvider>();

        return services;
    }
}
