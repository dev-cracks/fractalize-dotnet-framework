namespace DevCracks.Fractalize.Domain.Events;

/// <summary>
/// Interface for domain events.
/// This interface serves as a marker for events that occur within the domain.
/// It does not define any members, allowing for flexibility in the implementation of domain events.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// The time when the event was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get;  }

    /// <summary>
    /// A unique identifier for the event.
    /// This ID is generated as a new GUID by default, but can be set to a specific value.
    /// It is used to uniquely identify the event within the system.
    /// </summary>
    public string EventId { get; set; }

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
}
