using DevCracks.Fractalize.Domain.Commands;
using DevCracks.Fractalize.Domain.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DevCracks.Fractalize.Application.Commands;

/// <summary>
/// Extension methods for configuring command dispatchers and handlers in the application.
/// This class provides methods to register command dispatchers and handlers in the dependency injection container.
/// It uses the Microsoft.Extensions.DependencyInjection library to scan for classes that implement the ICommandHandler interface
/// and registers them with a transient lifetime.
/// The AddCommandDispatchers method is the main entry point for adding command dispatchers to the service collection.
/// It scans the application dependencies for classes that implement ICommandDispatcher and registers them as their implemented interfaces.
/// The TryAddCommandDispatcher method allows for the registration of a specific command dispatcher for a given command type.
/// The AddCommandHandlers method registers all command handlers in the application dependencies,
/// ensuring that command handlers can be resolved and used to process commands.
/// The ConfigureCommandHandler method allows for the registration of a specific command handler for a given command type,
/// ensuring that the command type implements the ICommand interface and that the command dispatcher is registered.
/// This class is part of the DevCracks.Fractalize.Application.Commands namespace, which contains classes and interfaces related to command handling.
/// It is typically used in the ConfigureServices method of the Startup class to set up command handling in the application.
/// The command dispatchers and handlers are registered with a transient lifetime, meaning a new instance will be created each time one is requested.
/// The methods in this class are designed to be used in conjunction with the Microsoft.Extensions.DependencyInjection library,
/// allowing for easy integration of command handling into the application's dependency injection system.
/// The command dispatchers and handlers are key components in the command handling infrastructure of an application,
/// enabling commands to be processed asynchronously by their respective handlers.
/// This class provides a convenient way to set up command handling in an application,
/// allowing commands to be dispatched to their respective handlers and processed according to the application's business logic.
/// It is designed to be used in conjunction with the DevCracks.Fractalize.Domain.Commands namespace,
/// which contains the ICommand interface and other related command handling interfaces.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Registers all command handlers in the application dependencies.
    /// This method scans the application dependencies for classes that implement the ICommandHandler interface
    /// and registers them as transient services in the IServiceCollection.
    /// It uses the Microsoft.Extensions.DependencyInjection.Extensions namespace to add the services.
    /// The command handlers are expected to handle commands of type ICommand.
    /// This method is typically called in the ConfigureServices method of the Startup class to set up command handling in the application.
    /// The command handlers are registered with a transient lifetime, meaning a new instance will be created each time one is requested.
    /// The method does not return any value, it modifies the IServiceCollection passed as a parameter.
    /// This method is useful for setting up the command handling infrastructure in an application,
    /// allowing commands to be dispatched to their respective handlers.
    /// It is part of the DevCracks.Fractalize.Application.Commands namespace, which contains classes and interfaces related to command handling.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCommandDispatchers(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromApplicationDependencies()
            .AddClasses(c => c.AssignableTo(typeof(IQueryDispatcher<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return services;
    }

    /// <summary>
    /// Registers a command dispatcher for a specific command type.
    /// This method attempts to add a transient service of type ICommandDispatcher<TCommand> to the IServiceCollection.
    /// It uses the TryAddTransient method from the Microsoft.Extensions.DependencyInjection.Extensions namespace,
    /// which will only add the service if it has not already been registered.
    /// The TCommand type parameter must implement the ICommand interface, ensuring that the command dispatcher can handle commands of that type.
    /// This method is typically used in the ConfigureServices method of the Startup class to set up command dispatching for a specific command type.
    /// The command dispatcher is responsible for finding the appropriate command handler for the given command type
    /// and invoking its HandleAsync method.
    /// The method returns the IServiceCollection to allow for method chaining in the configuration process.
    /// It is part of the DevCracks.Fractalize.Application.Commands namespace, which contains classes and interfaces related to command handling.
    /// It is useful for setting up the command handling infrastructure in an application,
    /// allowing commands to be dispatched to their respective handlers.
    /// It is designed to be used in conjunction with the AddCommandHandlers method, which registers all command handlers in the application dependencies.
    /// The command dispatcher is a key component in the command handling infrastructure of an application,
    /// enabling commands to be processed asynchronously by their respective handlers.
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection TryAddCommandDispatcher<TCommand>(this IServiceCollection services)
        where TCommand : ICommand
    {
        services.TryAddTransient<ICommandDispatcher<TCommand>, CommandDispatcher<TCommand>>();

        return services;
    }

    /// <summary>
    /// Registers all command handlers in the application dependencies.
    /// This method scans the application dependencies for classes that implement the ICommandHandler interface
    /// and registers them as transient services in the IServiceCollection.
    /// It uses the Microsoft.Extensions.DependencyInjection.Extensions namespace to add the services.
    /// The command handlers are expected to handle commands of type ICommand.
    /// This method is typically called in the ConfigureServices method of the Startup class to set up command handling in the application.
    /// The command handlers are registered with a transient lifetime, meaning a new instance will be created each time one is requested.
    /// The method does not return any value, it modifies the IServiceCollection passed as a parameter.
    /// This method is useful for setting up the command handling infrastructure in an application,
    /// allowing commands to be dispatched to their respective handlers.
    /// It is part of the DevCracks.Fractalize.Application.Commands namespace, which contains classes and interfaces related to command handling.
    /// The command handlers are registered with a transient lifetime, meaning a new instance will be created each time one is requested.
    /// The method also calls AddCommandDispatchers to ensure that command dispatchers are registered.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromApplicationDependencies()
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.AddCommandDispatchers();
        return services;
    }

    /// <summary>
    /// Registers a command handler for a specific command type.
    /// This method attempts to add a transient service of type ICommand to the IServiceCollection,
    /// ensuring that the command type TCommand implements the ICommand interface.
    /// It then calls TryAddCommandDispatcher<TCommand>() to register the command dispatcher for the specified command type.
    /// This method is typically used in the ConfigureServices method of the Startup class to set up command handling for a specific command type.
    /// The command handler is responsible for processing commands of type TCommand and executing the associated business logic.
    /// The method returns the IServiceCollection to allow for method chaining in the configuration process.
    /// It is part of the DevCracks.Fractalize.Application.Commands namespace, which contains classes and interfaces related to command handling.
    /// It is useful for setting up the command handling infrastructure in an application,
    /// allowing commands to be dispatched to their respective handlers.
    /// It is designed to be used in conjunction with the AddCommandHandlers method, which registers all command handlers in the application dependencies.
    /// The command handler is a key component in the command handling infrastructure of an application,
    /// enabling commands to be processed asynchronously by their respective handlers.
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>     
    public static IServiceCollection ConfigureCommandHandler<TCommand>(this IServiceCollection services)
        where TCommand : ICommand
    {
        services.TryAddTransient<ICommand>();
        services.TryAddCommandDispatcher<TCommand>();
        return services;
    }
}
