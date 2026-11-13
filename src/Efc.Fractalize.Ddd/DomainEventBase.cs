namespace Efc.Fractalize.Ddd;

/// <summary>
/// Base class for domain events: something meaningful that happened in the domain, recorded by an
/// <see cref="AggregateRootBase{TId}"/>.
/// </summary>
/// <remarks>
/// Overlaps with <c>Efc.Fractalize.Domain.Events.IDomainEvent and EventBase</c>
/// in the <c>Efc.Fractalize.Domain</c> package. Both are kept until the maintainers decide which one
/// survives; do not mix them in the same model.
/// </remarks>
public abstract class DomainEventBase
{
    /// <summary>
    /// Name of the event.
    /// </summary>
    public abstract required string Name { get; set; }

    /// <summary>
    /// When the event happened (ISO 8601 when serialized). Take it from an injected
    /// <see cref="TimeProvider"/> (<c>timeProvider.GetUtcNow()</c>), never from the system clock directly, so
    /// that it can be controlled in tests.
    /// </summary>
    public abstract required DateTimeOffset Date { get; set; }
}
