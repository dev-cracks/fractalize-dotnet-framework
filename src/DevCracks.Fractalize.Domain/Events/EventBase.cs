namespace DevCracks.Fractalize.Domain.Events;

/// <summary>
/// Interface for domain events.
/// This interface serves as a marker for events that occur within the domain.
/// It does not define any members, allowing for flexibility in the implementation of domain events.
/// </summary>
public abstract class EventBase : IDomainEvent
{
    /// <summary>
    /// The time when the event was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// A unique identifier for the event.
    /// This ID is generated as a new GUID by default, but can be set to a specific value.
    /// It is used to uniquely identify the event within the system.
    /// </summary>
    public string EventId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// A unique identifier for correlating related events.
    /// This can be used to track the flow of events across different parts of the system.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// A unique identifier for the causation of the event.
    /// This can be used to trace back to the event that caused this event to occur.
    /// </summary>
    public string? CausationId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventBase{EnumType}"/> class.
    /// This constructor is used to create an event without any specific parameters.
    /// </summary>
    protected EventBase()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventBase{EnumType}"/> class with a specified creation time and event ID.
    /// This constructor is used to create an event with a specific creation time and identifier,
    /// optionally including correlation and causation IDs.
    /// </summary>
    public EventBase(DateTimeOffset createdAt, string eventId, string? correlationId = null, string? causationId = null)
    {
        CreatedAt = createdAt;
        EventId = eventId;
        CorrelationId = correlationId;
        CausationId = causationId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventBase{EnumType}"/> class with a specified event ID.
    /// This constructor is used to create an event with a specific identifier, optionally including correlation and causation IDs.
    /// </summary>
    public EventBase(string eventId, string? correlationId = null, string? causationId = null)
    {
        EventId = eventId;
        CorrelationId = correlationId;
        CausationId = causationId;
    }

    /// <summary>
    /// Returns a string representation of the event.
    /// This method overrides the default ToString method to provide a meaningful representation of the event,
    /// typically including the event type, event ID, creation time, correlation ID, and causation ID.
    /// </summary>
    /// <returns></returns>
    public override string ToString() =>
        $"{EventId} - {CreatedAt:O} - CorrelationId: {CorrelationId} - CausationId: {CausationId}";

    /// <summary>
    /// Determines whether the specified event is equal to the current event.
    /// This method implements the IEquatable interface to provide a strongly-typed comparison.
    /// It checks if the other event is not null and compares their properties for equality.
    /// </summary>
    public override int GetHashCode() =>
        HashCode.Combine(EventId, CreatedAt, CorrelationId, CausationId);
}
