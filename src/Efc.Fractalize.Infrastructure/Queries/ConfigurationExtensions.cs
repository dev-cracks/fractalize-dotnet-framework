using Efc.Fractalize.Domain.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scrutor;

namespace Efc.Fractalize.Infrastructure.Queries;

/// <summary>
/// Registration helpers of version 2.x for query dispatchers and handlers.
/// </summary>
/// <remarks>
/// Kept for backward compatibility with unchanged behaviour. New code uses
/// <see cref="QueryDispatcherConfigurator"/>.
/// </remarks>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Registers, with a transient lifetime and as their implemented interfaces, the classes of every application
    /// dependency that implement <see cref="IQueryDispatcher{TQuery, TResult}"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection.</returns>
    [Obsolete("Use QueryDispatcherConfigurator.AddQueryDispatcher instead.")]
    public static IServiceCollection AddQueryDispatchers(this IServiceCollection services) =>
        services.AddTransientImplementations(typeof(IQueryDispatcher<,>), [], RegistrationStrategy.Append);

    /// <summary>
    /// Registers the query dispatcher for <typeparamref name="TQuery"/> if none is registered yet.
    /// </summary>
    /// <typeparam name="TQuery">The query type.</typeparam>
    /// <typeparam name="TResult">The result type of the query.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection.</returns>
    [Obsolete("Use QueryDispatcherConfigurator.AddQueryDispatcher<TQuery, TResult> instead.")]
    public static IServiceCollection TryAddQueryDispatcher<TQuery, TResult>(this IServiceCollection services)
        where TQuery : IQuery<TResult> =>
        services.AddQueryDispatcher<TQuery, TResult>();

    /// <summary>
    /// Registers, with a transient lifetime, every <see cref="IQueryHandler{TQuery, TResult}"/> implementation of
    /// the application dependencies, then calls <see cref="AddQueryDispatchers(IServiceCollection)"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection.</returns>
    [Obsolete("Use QueryDispatcherConfigurator.AddQueryDispatcher instead.")]
    public static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
        services
            .AddTransientImplementations(typeof(IQueryHandler<,>), [], RegistrationStrategy.Append)
            .AddQueryDispatchers();

    /// <summary>
    /// Registers <see cref="IQuery{TResult}"/> as a transient service and the query dispatcher for
    /// <typeparamref name="TQuery"/>.
    /// </summary>
    /// <typeparam name="TQuery">The query type.</typeparam>
    /// <typeparam name="TResult">The result type of the query.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection.</returns>
    [Obsolete("Use QueryDispatcherConfigurator.AddQueryDispatcher<TQuery, TResult> instead.")]
    public static IServiceCollection ConfigureQueryHandler<TQuery, TResult>(this IServiceCollection services)
        where TQuery : IQuery<TResult>
    {
        services.TryAddTransient<IQuery<TResult>>();
        return services.AddQueryDispatcher<TQuery, TResult>();
    }
}
