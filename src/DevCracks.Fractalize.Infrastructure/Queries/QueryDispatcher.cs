using DevCracks.Fractalize.Domain.Queries;

namespace DevCracks.Fractalize.Infrastructure.Queries;

/// <summary>
/// QueryDispatcher is responsible for dispatching queries to their respective handlers.
/// It implements the IQueryDispatcher interface, allowing for asynchronous query handling.
/// </summary>
/// <remarks>
/// Initializes a new instance of the QueryDispatcher class.
/// </remarks>
/// <param name="serviceProvider">The service provider used to resolve query handlers.</param>
public class QueryDispatcher<TQuery, TResult>(IServiceProvider provider) : IQueryDispatcher<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    /// <summary>
    /// Dispatches a query to its handler asynchronously.
    /// This method resolves the appropriate query handler for the given query type
    /// and invokes its HandleAsync method to process the query.
    /// The method uses reflection to find the handler type and invoke the HandleAsync method,
    /// allowing for a flexible and decoupled architecture.
    /// It is designed to handle queries that return a result of type TResult.
    /// The method takes a query of type TQuery and an optional cancellation token,
    /// which can be used to cancel the operation if needed.
    /// The method returns a Task of type TResult, representing the result of the query handling operation.
    /// If no handler is found for the query type, an InvalidOperationException is thrown.
    /// If the handler does not implement the HandleAsync method, an InvalidOperationException is also thrown.
    /// This setup is useful for implementing the CQRS (Command Query Responsibility Segregation) pattern in the application,
    /// enabling better organization and scalability of the codebase.
    /// The QueryDispatcher class is intended to be used in applications that follow the CQRS pattern,
    /// where queries are separated from commands, allowing for a clear distinction between read and write operations.
    /// It allows for the execution of queries in a decoupled manner, enabling better organization and scalability of the codebase.
    /// The method is intended to be called during the application runtime to execute queries and retrieve results.
    /// It is designed to be used in applications that follow the CQRS (Command Query Responsibility Segregation) pattern,
    /// where queries are separated from commands, allowing for a clear distinction between read and write operations.
    /// The QueryDispatcher class is intended to be used in applications that follow the CQRS pattern,
    /// where queries are separated from commands, allowing for a clear distinction between read and write operations.  
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<TResult> DispatchAsync(TQuery query, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        var handler = provider.GetService(handlerType) ?? throw new InvalidOperationException($"No handler registered for query type {query.GetType().Name}");
        var method = handlerType.GetMethod("HandleAsync") ?? throw new InvalidOperationException($"Handler {handler.GetType().Name} does not have HandleAsync method");
        var task = (Task<TResult>)method.Invoke(handler, new object?[] { query, cancellationToken })!;
        return await task;
    }
}
