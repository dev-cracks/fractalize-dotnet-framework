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
| `DomainEventBase` | Base class for domain events, with init-only `Name` and `Date` |
| `ValueObjectBase` | Base record for value objects (record value equality) |
| `DateRange` | Value object for a period: validation, `Overlaps` and duration in minutes |
| `IReadRepository<TEntity, TId>` | Read access: `GetAllAsync`, `WhereAsync` and `GetByIdAsync` |
| `IRepository<TEntity, TId>` | Marker for the repository of an aggregate root |
| `IUnitOfWorkBase` | `SaveChangesAsync` to commit the tracked changes |

## Example

```csharp
using Efc.Fractalize.Ddd;

public sealed class OrderPlaced : DomainEventBase
{
    public override required string Name { get; init; }

    public override required DateTimeOffset Date { get; init; }
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

## Relation with `Efc.Fractalize.Domain`

This package is the common DDD base of the framework: `Efc.Fractalize.Domain` depends on it and no longer declares
its own entities, aggregate roots or domain events. The
[migration table](https://github.com/dev-cracks/fractalize-dotnet-framework#migrating-the-ddd-building-blocks-of-efcfractalizedomain)
maps the removed types to the ones of this package.

## Origin

This package comes from `dev-cracks/fractalize-dotnet-ddd` (formerly `ddd-csharp`, namespace
`Fractalize.Ddd.SharedKernel`). When it was integrated, these bugs were fixed:

- `ClearDomainEvents()` empties the list (it used to add the event it received).
- The entity `Id` is init-only and entities are compared by identity.
- The custom `IClockStamp` abstraction is replaced by `System.TimeProvider`.
- `DateRange.Overlaps` takes the end of the other range into account.
- The obsolete `ValueObjectLegacyBase` is removed: use `ValueObjectBase`.

## License

[MIT](https://github.com/dev-cracks/fractalize-dotnet-framework/blob/main/LICENSE)
