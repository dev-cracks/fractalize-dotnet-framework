namespace DevCracks.Fractalize.Domain.Base;

public abstract class EventBase<EnumType> where EnumType : struct, Enum
{
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public EnumType EventType { get; set; }
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public string? CorrelationId { get; set; }
    public string? CausationId { get; set; }
}
