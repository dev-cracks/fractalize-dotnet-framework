# Efc.Fractalize.Ddd

Domain-driven design core of the [Fractalize .NET Framework](https://github.com/dev-cracks/fractalize-dotnet-framework):
base types for entities, aggregate roots, domain events and value objects, plus the repository and unit of
work contracts. It has no dependencies and targets `net10.0`.

```bash
dotnet add package Efc.Fractalize.Ddd
```

## Types

| Type | Purpose |
| --- | --- |
| `EntityBase<TId>` | Entity with an init-only `Id` and identity equality (`Equals`, `GetHashCode`, `==`, `!=`). Transient entities (default `Id`) are only equal to themselves |
| `AggregateRootBase<TId>` | Entity that records domain events: `AddDomainEvent`, `DomainEvents` and `ClearDomainEvents()` |
| `DomainEventBase` | Base class for domain events, with `Name` and `Date` |
| `ValueObjectBase` | Base record for value objects (record value equality) |
| `ValueObjectLegacyBase` | Obsolete class-based value object with `IComparable`; use `ValueObjectBase` |
| `DateRange` | Value object for a period: validation, `Overlaps` and duration in minutes |
| `IReadRepository<TEntity, TId>` | Read access: `GetAllAsync`, `WhereAsync` and `GetByIdAsync` |
| `IRepository<TEntity, TId>` | Marker for the repository of an aggregate root |
| `IUnitOfWorkBase` | `SaveChangesAsync` to commit the tracked changes |

## Example

```csharp
using Efc.Fractalize.Ddd;

public sealed class OrderPlaced : DomainEventBase
{
    public override required string Name { get; set; }

    public override required DateTimeOffset Date { get; set; }
}

public sealed class Order : AggregateRootBase<Guid>
{
    public static Order Place(Guid id, TimeProvider timeProvider)
    {
        var order = new Order { Id = id };
        order.AddDomainEvent(new OrderPlaced { Name = "order-placed", Date = timeProvider.GetUtcNow() });
        return order;
    }
}

// After dispatching the events:
order.ClearDomainEvents();
```

Take time from an injected `TimeProvider` (`TimeProvider.System` in production, a fake in tests), never from
`DateTimeOffset.UtcNow`.

## Overlap with `Efc.Fractalize.Domain`

`Efc.Fractalize.Domain` also declares `EntityBase<TId>`, `AggregateRootBase<TId>` and domain event types
(`IDomainEvent`, `EventBase`) in other namespaces. Both packages are kept for now, until the maintainers
decide which types survive. Do not mix the two families in the same model.

## Origin

This package comes from `dev-cracks/fractalize-dotnet-ddd` (formerly `ddd-csharp`, namespace
`Fractalize.Ddd.SharedKernel`). When it was integrated, these bugs were fixed:

- `ClearDomainEvents()` empties the list (it used to add the event it received).
- The entity `Id` is init-only and entities are compared by identity.
- The custom `IClockStamp` abstraction is replaced by `System.TimeProvider`.
- `DateRange.Overlaps` takes the end of the other range into account.

## License

[MIT](https://github.com/dev-cracks/fractalize-dotnet-framework/blob/main/LICENSE)
