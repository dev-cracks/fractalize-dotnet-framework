using DevCracks.Fractalize.Domain.Commands;

namespace DevCracks.Fractalize.Application.Commands;

/// <summary>
/// CommandDispatcher is responsible for dispatching commands to their respective handlers.
/// It uses the IServiceProvider to resolve the appropriate command handler based on the command type.
/// It implements the ICommandDispatcher interface, which defines the DispatchAsync method.
/// The DispatchAsync method takes a command of type TCommand and an optional CancellationToken.
/// It uses reflection to find the appropriate command handler and invoke its HandleAsync method.
/// </summary>
/// <param name="provider"></param>
public class CommandDispatcher<TCommand>(IServiceProvider provider) : ICommandDispatcher<TCommand> where TCommand : ICommand
{
    /// <summary>
    /// Dispatches a command to its handler asynchronously.
    /// This method uses reflection to find the appropriate command handler for the given command type.
    /// It retrieves the handler type from the IServiceProvider and invokes its HandleAsync method.
    /// If no handler is found or if the handler does not have a HandleAsync method, it throws an InvalidOperationException.
    /// The method is generic and works with any command type that implements the ICommand interface.
    /// The cancellationToken parameter allows for cancellation of the operation.
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task DispatchAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        var handler = provider.GetService(handlerType) ?? throw new InvalidOperationException($"No handler registered for command type {command.GetType().Name}");
        var method = handlerType.GetMethod("HandleAsync") ?? throw new InvalidOperationException($"Handler {handler.GetType().Name} does not have HandleAsync method");

        await (Task)method.Invoke(handler, new object?[] { command, cancellationToken })!;
    }
}
