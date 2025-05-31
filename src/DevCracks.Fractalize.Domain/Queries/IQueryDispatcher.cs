namespace DevCracks.Fractalize.Domain.Queries;

/// <summary>
/// Interface for query dispatchers in the application layer.
/// Query dispatchers are responsible for handling queries and executing the appropriate query handlers.
/// They serve as a bridge between the application layer and the query handlers,
/// allowing queries to be processed in a decoupled manner.
/// </summary>
public interface IQueryDispatcher<TQuery, TResult> where TQuery : IQuery<TResult>
{
    /// <summary>
    /// Dispatches a query to the appropriate query handler for processing.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query to dispatch.</typeparam>
    /// <typeparam name="TResult">The type of the result returned by the query handler.</typeparam>
    /// <param name="query">The query to dispatch.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the query.</returns>
    Task<TResult> DispatchAsync(TQuery query, CancellationToken cancellationToken = default);
}
