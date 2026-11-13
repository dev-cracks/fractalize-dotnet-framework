using System.Reflection;
using Efc.Fractalize.Domain.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scrutor;

namespace Efc.Fractalize.Infrastructure.Queries;

/// <summary>
/// Fluent configurator of the query dispatcher: registers <see cref="IQueryDispatcher{TQuery, TResult}"/> with its
/// default implementation, <see cref="QueryDispatcher{TQuery, TResult}"/>, and the query handlers it dispatches to.
/// </summary>
/// <remarks>
/// Every registration uses <c>TryAdd*</c> semantics: a host can register its own implementation before or after
/// calling the configurator, and calling it twice is harmless.
/// </remarks>
public static class QueryDispatcherConfigurator
{
    /// <summary>
    /// Registers the query dispatcher for every query type and the query handlers found in
    /// <paramref name="handlerAssemblies"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="handlerAssemblies">
    /// The assemblies scanned for <see cref="IQueryHandler{TQuery, TResult}"/> implementations. When empty, every
    /// assembly the application depends on is scanned.
    /// </param>
    /// <returns>The same service collection, so calls can be chained.</returns>
    public static IServiceCollection AddQueryDispatcher(
        this IServiceCollection services, params Assembly[] handlerAssemblies)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(handlerAssemblies);

        services.TryAdd(ServiceDescriptor.Transient(typeof(IQueryDispatcher<,>), typeof(QueryDispatcher<,>)));
        return services.AddTransientImplementations(
            typeof(IQueryHandler<,>), handlerAssemblies, RegistrationStrategy.Skip);
    }

    /// <summary>
    /// Registers the query dispatcher for <typeparamref name="TQuery"/>.
    /// </summary>
    /// <typeparam name="TQuery">The query type.</typeparam>
    /// <typeparam name="TResult">The result type of the query.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection, so calls can be chained.</returns>
    public static IServiceCollection AddQueryDispatcher<TQuery, TResult>(this IServiceCollection services)
        where TQuery : IQuery<TResult>
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddTransient<IQueryDispatcher<TQuery, TResult>, QueryDispatcher<TQuery, TResult>>();
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
