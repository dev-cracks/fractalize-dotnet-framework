namespace Efc.Fractalize.Ddd;

/// <summary>
/// Base class for aggregate roots: entities that own a consistency boundary and record the domain events
/// raised while they change.
/// </summary>
/// <typeparam name="TId">Type of the identifier.</typeparam>
public abstract class AggregateRootBase<TId> : EntityBase<TId>
{
    private readonly List<DomainEventBase> _domainEvents = [];

    /// <summary>
    /// Domain events raised and not yet dispatched, in the order they were added.
    /// </summary>
    public IReadOnlyCollection<DomainEventBase> DomainEvents => _domainEvents;

    /// <summary>
    /// Records a domain event raised by the aggregate.
    /// </summary>
    /// <param name="domainEvent">The event to record.</param>
    public void AddDomainEvent(DomainEventBase domainEvent) =>
        _domainEvents.Add(domainEvent);

    /// <summary>
    /// Removes every pending domain event, typically after they have been dispatched.
    /// </summary>
    public void ClearDomainEvents() =>
        _domainEvents.Clear();
}
