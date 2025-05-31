namespace DevCracks.Fractalize.Domain.Queries;

/// <summary>
/// Interface for queries in the domain.
/// Queries are used to retrieve data or information from the domain
/// without modifying the state of the system.
/// </summary>
/// <typeparam name="TQuery"></typeparam>
/// <typeparam name="TResult"></typeparam>
public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    where TResult : class
{
    /// <summary>
    /// Handles the specified query and returns the result.
    /// </summary>
    /// <param name="query">The query to handle.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the query.</returns>
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
