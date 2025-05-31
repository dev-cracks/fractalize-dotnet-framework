namespace DevCracks.Fractalize.Domain.Commands;

/// <summary>
/// Interface for commands in the domain.
/// Commands are used to perform actions or operations in the domain,
/// typically resulting in changes to the state of the system.
/// </summary>
public interface ICommandHandler <TCommand> where TCommand : ICommand
{
    /// <summary>
    /// Handles the command asynchronously.
    /// This method is responsible for executing the logic associated with the command,
    /// which may include validating the command, performing operations, and updating the state of the system.
    /// The method should be implemented to handle the specific command type.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
