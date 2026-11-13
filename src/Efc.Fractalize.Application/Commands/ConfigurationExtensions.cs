using Efc.Fractalize.Domain.Commands;
using Efc.Fractalize.Domain.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scrutor;

namespace Efc.Fractalize.Application.Commands;

/// <summary>
/// Registration helpers of version 2.x for command dispatchers and handlers.
/// </summary>
/// <remarks>
/// Kept for backward compatibility with unchanged behaviour. New code uses
/// <see cref="CommandDispatcherConfigurator"/>.
/// </remarks>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Registers, with a transient lifetime and as their implemented interfaces, the classes of every application
    /// dependency that implement <see cref="IQueryDispatcher{TQuery, TResult}"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection.</returns>
    [Obsolete("Use CommandDispatcherConfigurator.AddCommandDispatcher instead.")]
    public static IServiceCollection AddCommandDispatchers(this IServiceCollection services) =>
        services.AddTransientImplementations(typeof(IQueryDispatcher<,>), [], RegistrationStrategy.Append);

    /// <summary>
    /// Registers the command dispatcher for <typeparamref name="TCommand"/> if none is registered yet.
    /// </summary>
    /// <typeparam name="TCommand">The command type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection.</returns>
    [Obsolete("Use CommandDispatcherConfigurator.AddCommandDispatcher<TCommand> instead.")]
    public static IServiceCollection TryAddCommandDispatcher<TCommand>(this IServiceCollection services)
        where TCommand : ICommand =>
        services.AddCommandDispatcher<TCommand>();

    /// <summary>
    /// Registers, with a transient lifetime, every <see cref="ICommandHandler{TCommand}"/> implementation of the
    /// application dependencies, then calls <see cref="AddCommandDispatchers(IServiceCollection)"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection.</returns>
    [Obsolete("Use CommandDispatcherConfigurator.AddCommandDispatcher instead.")]
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
        services
            .AddTransientImplementations(typeof(ICommandHandler<>), [], RegistrationStrategy.Append)
            .AddCommandDispatchers();

    /// <summary>
    /// Registers <see cref="ICommand"/> as a transient service and the command dispatcher for
    /// <typeparamref name="TCommand"/>.
    /// </summary>
    /// <typeparam name="TCommand">The command type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection.</returns>
    [Obsolete("Use CommandDispatcherConfigurator.AddCommandDispatcher<TCommand> instead.")]
    public static IServiceCollection ConfigureCommandHandler<TCommand>(this IServiceCollection services)
        where TCommand : ICommand
    {
        services.TryAddTransient<ICommand>();
        return services.AddCommandDispatcher<TCommand>();
    }
}
