using System.Reflection;
using Efc.Fractalize.Domain.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scrutor;

namespace Efc.Fractalize.Application.Commands;

/// <summary>
/// Fluent configurator of the command dispatcher: registers <see cref="ICommandDispatcher{TCommand}"/> with its
/// default implementation, <see cref="CommandDispatcher{TCommand}"/>, and the command handlers it dispatches to.
/// </summary>
/// <remarks>
/// Every registration uses <c>TryAdd*</c> semantics: a host can register its own implementation before or after
/// calling the configurator, and calling it twice is harmless.
/// </remarks>
public static class CommandDispatcherConfigurator
{
    /// <summary>
    /// Registers the command dispatcher for every command type and the command handlers found in
    /// <paramref name="handlerAssemblies"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="handlerAssemblies">
    /// The assemblies scanned for <see cref="ICommandHandler{TCommand}"/> implementations. When empty, every
    /// assembly the application depends on is scanned.
    /// </param>
    /// <returns>The same service collection, so calls can be chained.</returns>
    public static IServiceCollection AddCommandDispatcher(
        this IServiceCollection services, params Assembly[] handlerAssemblies)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(handlerAssemblies);

        services.TryAdd(ServiceDescriptor.Transient(typeof(ICommandDispatcher<>), typeof(CommandDispatcher<>)));
        return services.AddTransientImplementations(
            typeof(ICommandHandler<>), handlerAssemblies, RegistrationStrategy.Skip);
    }

    /// <summary>
    /// Registers the command dispatcher for <typeparamref name="TCommand"/>.
    /// </summary>
    /// <typeparam name="TCommand">The command type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection, so calls can be chained.</returns>
    public static IServiceCollection AddCommandDispatcher<TCommand>(this IServiceCollection services)
        where TCommand : ICommand
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddTransient<ICommandDispatcher<TCommand>, CommandDispatcher<TCommand>>();
        return services;
    }

    /// <summary>
    /// Registers, with a transient lifetime and as their implemented interfaces, the classes of
    /// <paramref name="assemblies"/> (every application dependency when empty) assignable to
    /// <paramref name="serviceType"/>.
    /// </summary>
    internal static IServiceCollection AddTransientImplementations(
        this IServiceCollection services,
        Type serviceType,
        Assembly[] assemblies,
        RegistrationStrategy strategy)
    {
        services.Scan(scan => (assemblies.Length == 0 ? scan.FromApplicationDependencies() : scan.FromAssemblies(assemblies))
            .AddClasses(c => c.AssignableTo(serviceType))
            .UsingRegistrationStrategy(strategy)
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return services;
    }
}
