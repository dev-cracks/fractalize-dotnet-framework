using DevCracks.Fractalize.Domain.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace DevCracks.Fractalize.Application.Extensions;

/// <summary>
/// Extension methods for configuring CQRS in the application.
/// This class provides methods to register command handlers and other related services in the dependency injection container.
/// It uses the Microsoft.Extensions.DependencyInjection library to scan for classes that implement the ICommandHandler interface
/// and registers them with a transient lifetime.
/// The AddCqrs method is the main entry point for adding CQRS services to the service collection.
/// It scans the application dependencies for classes that implement ICommandHandler and registers them as their implemented interfaces.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Registers CQRS services in the service collection.
    /// This method scans the application dependencies for classes that implement the ICommandHandler interface
    /// and registers them with a transient lifetime.
    /// It is intended to be called during the application startup to configure the CQRS infrastructure.
    /// The method uses the Microsoft.Extensions.DependencyInjection library to perform the scanning and registration.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromApplicationDependencies()
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return services;
    }
}
