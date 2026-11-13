namespace Efc.Fractalize.Ddd;

/// <summary>
/// Unit of work: commits the changes tracked by the repositories in one transaction.
/// </summary>
public interface IUnitOfWorkBase : IDisposable
{
    /// <summary>
    /// Saves the tracked changes.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The number of changes written.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
