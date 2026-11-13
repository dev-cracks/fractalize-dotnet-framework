namespace Efc.Fractalize.Ddd;

/// <summary>
/// Base class for aggregate roots: entities that own a consistency boundary and record the domain events
/// raised while they change.
/// </summary>
/// <remarks>
/// <para>
/// Not thread-safe: an aggregate is meant to be changed by one operation at a time.
/// </para>
/// </remarks>
/// <typeparam name="TId">Type of the identifier.</typeparam>
public abstract class AggregateRootBase<TId> : EntityBase<TId>
{
    private readonly List<DomainEventBase> _domainEvents = [];

    /// <summary>
    /// Domain events raised and not yet dispatched, in the order they were added. It is a read-only view:
    /// callers cannot change the list through it.
    /// </summary>
    public IReadOnlyCollection<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Records a domain event raised by the aggregate.
    /// </summary>
    /// <param name="domainEvent">The event to record.</param>
    /// <exception cref="ArgumentNullException"><paramref name="domainEvent"/> is <see langword="null"/>.</exception>
    public void AddDomainEvent(DomainEventBase domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Removes every pending domain event, typically after they have been dispatched.
    /// </summary>
    public void ClearDomainEvents() =>
        _domainEvents.Clear();
}
