using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Efc.Fractalize.Infrastructure.Extensions;

/// <summary>
/// Fluent configurator of strongly typed options: binds them, validates their data annotations and fails at
/// startup when they are invalid.
/// </summary>
public static class OptionsConfigurator
{
    /// <summary>
    /// Binds <typeparamref name="TOptions"/> to the configuration section <paramref name="sectionName"/> and
    /// validates its data annotations when the application starts.
    /// </summary>
    /// <typeparam name="TOptions">The options type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration that contains the section.</param>
    /// <param name="sectionName">The key of the configuration section, for example <c>Shipments:Repository</c>.</param>
    /// <returns>The same service collection, so calls can be chained.</returns>
    public static IServiceCollection AddValidatedOptions<TOptions>(
        this IServiceCollection services, IConfiguration configuration, string sectionName)
        where TOptions : class
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrWhiteSpace(sectionName);

        return services.AddValidatedOptions<TOptions>(configuration.GetSection(sectionName).Bind);
    }

    /// <summary>
    /// Configures <typeparamref name="TOptions"/> from code (tests, other hosts, values read from a database) and
    /// validates its data annotations when the application starts.
    /// </summary>
    /// <typeparam name="TOptions">The options type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">The action that sets the option values.</param>
    /// <returns>The same service collection, so calls can be chained.</returns>
    public static IServiceCollection AddValidatedOptions<TOptions>(
        this IServiceCollection services, Action<TOptions> configure)
        where TOptions : class
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        services.AddOptions<TOptions>()
            .Configure(configure)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
