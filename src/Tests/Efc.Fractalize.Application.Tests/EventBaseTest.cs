using Efc.Fractalize.Domain.Events;

namespace Efc.Fractalize.Application.Tests;

public class EventBaseTest
{
    [Fact]
    public void DefaultConstructor_GeneratesIdAndCurrentCreationTime()
    {
        var before = DateTimeOffset.UtcNow;

        var domainEvent = new TestEvent();

        Assert.True(Guid.TryParse(domainEvent.EventId, out _));
        Assert.InRange(domainEvent.CreatedAt, before, DateTimeOffset.UtcNow);
        Assert.Null(domainEvent.CorrelationId);
        Assert.Null(domainEvent.CausationId);
    }

    [Fact]
    public void Constructor_WithAllValues_KeepsThem()
    {
        var createdAt = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero);

        var domainEvent = new TestEvent(createdAt, "event-1", "correlation", "causation");

        Assert.Equal(createdAt, domainEvent.CreatedAt);
        Assert.Equal("event-1", domainEvent.EventId);
        Assert.Equal("correlation", domainEvent.CorrelationId);
        Assert.Equal("causation", domainEvent.CausationId);
        Assert.Equal("event-1 - 2026-01-02T03:04:05.0000000+00:00 - CorrelationId: correlation - CausationId: causation", domainEvent.ToString());
    }

    [Fact]
    public void Constructor_WithEventId_KeepsIdsAndHashesConsistently()
    {
        var domainEvent = new TestEvent("event-2", "correlation", "causation");

        Assert.Equal("event-2", domainEvent.EventId);
        Assert.Equal("correlation", domainEvent.CorrelationId);
        Assert.Equal("causation", domainEvent.CausationId);
        Assert.Equal(domainEvent.GetHashCode(), domainEvent.GetHashCode());
    }

    private sealed class TestEvent : EventBase
    {
        public TestEvent()
        {
        }

        public TestEvent(DateTimeOffset createdAt, string eventId, string? correlationId, string? causationId)
            : base(createdAt, eventId, correlationId, causationId)
        {
        }

        public TestEvent(string eventId, string? correlationId, string? causationId)
            : base(eventId, correlationId, causationId)
        {
        }
    }
}
