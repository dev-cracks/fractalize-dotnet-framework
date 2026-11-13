namespace Efc.Fractalize.Ddd;

public abstract class AggregateRootBase<TId> :  EntityBase<TId>
{
    private readonly List<DomainEventBase> _domainEvents = [];

    public IReadOnlyCollection<DomainEventBase> DomainEvents => _domainEvents;

    public void AddDomainEvent(DomainEventBase domainEvent) =>
        _domainEvents.Add(domainEvent);

    public void ClearDomainEvents(DomainEventBase domainEvent) =>
        _domainEvents.Add(domainEvent);
}
