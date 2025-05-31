using DevCracks.Fractalize.Domain.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DevCracks.Fractalize.Infrastructure.Queries;

/// <summary>
/// Extension methods for configuring query dispatchers and handlers in the service collection.
/// This class provides methods to register query dispatchers and handlers in the dependency injection container.
/// It uses the Microsoft.Extensions.DependencyInjection library to scan for classes that implement the IQueryDispatcher and IQueryHandler interfaces
/// and registers them with a transient lifetime.
/// The AddQueryDispatchers method is the main entry point for adding query dispatchers to the service collection.
/// The TryAddQueryDispatcher method allows for adding a specific query dispatcher if it is not already registered.
/// The AddQueryHandlers method scans for query handlers and registers them as well.
/// The ConfigureQueryHandler method allows for configuring a specific query handler by registering the query and its dispatcher.
/// This setup is useful for implementing the CQRS (Command Query Responsibility Segregation) pattern in the application.
/// It allows for separating the query logic from the command logic, enabling better organization and scalability of the codebase.
/// It is intended to be called during the application startup to configure the query infrastructure.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Adds query dispatchers to the service collection.
    /// This method scans the application dependencies for classes that implement the IQueryDispatcher interface
    /// and registers them with a transient lifetime.
    /// It is used to set up the query dispatching infrastructure in the application.
    /// The IQueryDispatcher interface is responsible for dispatching queries to their respective handlers,
    /// allowing for the execution of queries in a decoupled manner.
    /// This setup is useful for implementing the CQRS (Command Query Responsibility Segregation) pattern in the application,
    /// enabling better organization and scalability of the codebase.
    /// The method is intended to be called during the application startup to configure the query dispatchers.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddQueryDispatchers(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromApplicationDependencies()
            .AddClasses(c => c.AssignableTo(typeof(IQueryDispatcher<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return services;
    }

    /// <summary>
    ///  Tries to add a query dispatcher for a specific query type and result type.
    /// This method checks if a query dispatcher for the specified query type and result type is already registered.
    /// If it is not registered, it adds a transient service for the IQueryDispatcher interface
    /// for the specified query type and result type.
    /// This setup is useful for implementing the CQRS (Command Query Responsibility Segregation) pattern in the application,
    /// allowing for the separation of query logic from command logic.
    /// It enables better organization and scalability of the codebase by allowing queries to be dispatched to their respective handlers.
    /// </summary>
    /// <typeparam name="TQuery"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection TryAddQueryDispatcher<TQuery, TResult>(this IServiceCollection services)
        where TQuery : IQuery<TResult>
    {
        services.TryAddTransient<IQueryDispatcher<TQuery, TResult>, QueryDispatcher<TQuery, TResult>>();

        return services;
    }

    /// <summary>
    /// Registers query handlers in the service collection.
    /// This method scans the application dependencies for classes that implement the IQueryHandler interface
    /// and registers them with a transient lifetime.
    /// It is used to set up the query handling infrastructure in the application.
    /// The IQueryHandler interface is responsible for handling queries and returning results,
    /// allowing for the execution of queries in a decoupled manner.
    /// This setup is useful for implementing the CQRS (Command Query Responsibility Segregation) pattern in the application,
    /// enabling better organization and scalability of the codebase.
    /// The method is intended to be called during the application startup to configure the query handlers.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddQueryHandlers(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromApplicationDependencies()
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.AddQueryDispatchers();
        return services;
    }

    /// <summary>
    /// Configures a specific query handler for a given query type and result type.
    /// This method registers a transient service for the IQuery interface for the specified result type,
    /// and adds a query dispatcher for the specified query type and result type.
    /// It is used to set up the query handling infrastructure for a specific query type,
    /// allowing for the execution of queries in a decoupled manner.
    /// This setup is useful for implementing the CQRS (Command Query Responsibility Segregation) pattern in the application,
    /// enabling better organization and scalability of the codebase.
    /// The method is intended to be called during the application startup to configure the query handler for the specified query type.
    /// </summary>
    /// <typeparam name="TQuery"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns> 
    public static IServiceCollection ConfigureQueryHandler<TQuery, TResult>(this IServiceCollection services)
        where TQuery : IQuery<TResult>
    {
        services.TryAddTransient<IQuery<TResult>>();
        services.TryAddQueryDispatcher<TQuery, TResult>();
        return services;
    }
}
