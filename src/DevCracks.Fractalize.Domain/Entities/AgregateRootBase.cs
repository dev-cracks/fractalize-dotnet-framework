using DevCracks.Fractalize.Domain.Events;

namespace DevCracks.Fractalize.Domain.Entities;

/// <summary>
/// Base class for aggregate roots in the domain model.
/// Aggregate roots are the main entry point for interacting with a cluster of related entities
/// and are responsible for maintaining the consistency of the aggregate.
/// This class inherits from EntityBase and provides functionality to manage domain events.
/// It allows adding events to the aggregate root and clearing the list of events after they have been processed.   
/// </summary>
/// <typeparam name="TId"></typeparam>
public abstract class AggregateRootBase<TId> : EntityBase<TId>
    where TId : notnull
{
    /// <summary>
    /// Clears the list of events after they have been processed.
    /// </summary>
    public void ClearEvents() => _events.Clear();

    /// <summary>
    /// Adds an event to the aggregate root.
    /// </summary>
    /// <param name="event"></param>
    public void AddEvent(IDomainEvent @event) => _events.Add(@event);
}
