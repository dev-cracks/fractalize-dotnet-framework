using System.Reflection;
using Efc.Fractalize.Application.Commands;
using Efc.Fractalize.Infrastructure.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Efc.Fractalize.Infrastructure.Extensions;

/// <summary>
/// Fluent configurator of the CQRS building blocks: the command dispatcher and the query dispatcher, with the
/// handlers they dispatch to.
/// </summary>
public static class CqrsConfigurator
{
    /// <summary>
    /// Registers the command and query dispatchers and the handlers found in <paramref name="handlerAssemblies"/>,
    /// through <see cref="CommandDispatcherConfigurator.AddCommandDispatcher(IServiceCollection, Assembly[])"/> and
    /// <see cref="QueryDispatcherConfigurator.AddQueryDispatcher(IServiceCollection, Assembly[])"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="handlerAssemblies">
    /// The assemblies scanned for command and query handlers. When empty, every assembly the application depends
    /// on is scanned.
    /// </param>
    /// <returns>The same service collection, so calls can be chained.</returns>
    public static IServiceCollection AddCqrsDispatchers(
        this IServiceCollection services, params Assembly[] handlerAssemblies) =>
        services
            .AddCommandDispatcher(handlerAssemblies)
            .AddQueryDispatcher(handlerAssemblies);
}
