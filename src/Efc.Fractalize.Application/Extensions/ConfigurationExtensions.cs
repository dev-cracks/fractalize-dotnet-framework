using Efc.Fractalize.Application.Commands;
using Efc.Fractalize.Domain.Commands;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Efc.Fractalize.Application.Extensions;

/// <summary>
/// Registration helper of version 2.x for CQRS.
/// </summary>
/// <remarks>
/// Kept for backward compatibility with unchanged behaviour. New code uses <c>CqrsConfigurator.AddCqrsDispatchers</c>
/// (package <c>Efc.Fractalize.Infrastructure</c>) or <see cref="CommandDispatcherConfigurator"/>.
/// </remarks>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Registers, with a transient lifetime, every <see cref="ICommandHandler{TCommand}"/> implementation of the
    /// application dependencies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection.</returns>
    [Obsolete("Use CqrsConfigurator.AddCqrsDispatchers instead.")]
    public static IServiceCollection AddCqrs(this IServiceCollection services) =>
        services.AddTransientImplementations(typeof(ICommandHandler<>), [], RegistrationStrategy.Append);
}
