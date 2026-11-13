using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Efc.Fractalize.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring application settings in the service collection.
/// This class provides methods to add configuration sections to the service collection,
/// allowing for easy access to application settings throughout the application.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Adds a configuration section to the service collection.
    /// </summary>
    /// <typeparam name="T">The type of the configuration section.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="sectionName">The name of the configuration section.</param>
    /// <returns>The service collection with the added configuration section.</returns>
    /// <remarks>
    /// Kept for backward compatibility with unchanged behaviour: the section is bound only when it exists and is
    /// not validated. New code uses <see cref="OptionsConfigurator"/>, which validates at startup.
    /// </remarks>
    [Obsolete("Use OptionsConfigurator.AddValidatedOptions instead.")]
    public static IServiceCollection AddConfigurationSection<T>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
        where T : class, new()
    {
        var section = configuration.GetSection(sectionName);
        if (section.Exists())
        {
            services.Configure<T>(section);
        }
        return services;
    }
}
