namespace DevCracks.Fractalize.Domain.Commands;

/// <summary>
/// ICommandDispatcher is an interface that defines a contract for dispatching commands.
/// It is used to decouple the command dispatching logic from the command handling logic.
/// This interface allows for the implementation of different command dispatching strategies,
/// such as synchronous or asynchronous dispatching, and can be extended to support additional features like logging or error handling.
/// The ICommandDispatcher interface is generic and works with any command type that implements the ICommand interface.
/// It provides a single method, DispatchAsync, which takes a command of type TCommand and an optional CancellationToken.
/// The DispatchAsync method is responsible for finding the appropriate command handler for the given command type
/// and invoking its HandleAsync method.
/// The ICommandDispatcher interface is typically used in applications that follow the Command Query Responsibility Segregation (CQRS) pattern,
/// where commands are used to change the state of the application and are handled by command handlers.
/// It is part of the DevCracks.Fractalize.Domain.Commands namespace, which contains classes and interfaces related to command handling.
/// The ICommandDispatcher interface is designed to be implemented by classes that provide the actual command dispatching logic,
/// allowing for flexibility and extensibility in how commands are processed.
/// It is a key component in the command handling infrastructure of an application,
/// enabling commands to be dispatched to their respective handlers for processing.
/// </summary>
/// <typeparam name="TCommand"></typeparam>
public interface ICommandDispatcher<TCommand> where TCommand : ICommand
{
    /// <summary>
    /// Dispatches a command to the appropriate command handler for processing.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command to dispatch.</typeparam>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the command is null.</exception>
    Task DispatchAsync(TCommand command, CancellationToken cancellationToken = default);
}
