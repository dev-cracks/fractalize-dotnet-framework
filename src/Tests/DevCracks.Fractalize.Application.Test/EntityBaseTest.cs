using DevCracks.Fractalize.Domain.Entities;
using DevCracks.Fractalize.Domain.Events;

namespace Dev.Cracks.Fractalize.Application.Test;

public class EntityBaseTest
{
    [Fact]
    public void Equals_WithSameId_ReturnsTrue()
    {
        EntityBase<int> first = new TestAggregate(7);
        EntityBase<int> second = new TestAggregate(7);

        Assert.True(first.Equals(second));
        Assert.True(first.Equals((object)second));
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Equals_WithDifferentIdOrNull_ReturnsFalse()
    {
        EntityBase<int> entity = new TestAggregate(7);

        Assert.False(entity.Equals(new TestAggregate(8)));
        Assert.False(entity.Equals(null));
        Assert.False(entity.Equals((object?)null));
    }

    [Fact]
    public void ToString_ReturnsTypeNameAndId() =>
        Assert.Equal("TestAggregate [Id=7]", new TestAggregate(7).ToString());

    [Fact]
    public void DefaultConstructor_LeavesDefaultIdAndNoEvents()
    {
        var entity = new TestAggregate();

        Assert.Equal(0, entity.Id);
        Assert.Empty(entity.Events);
    }

    [Fact]
    public void AddEvent_ThenClearEvents_TracksAndRemovesEvents()
    {
        var aggregate = new TestAggregate(1);
        var domainEvent = new TestEvent("event-1");

        aggregate.AddEvent(domainEvent);
        Assert.Same(domainEvent, Assert.Single(aggregate.Events));

        aggregate.ClearEvents();
        Assert.Empty(aggregate.Events);
    }

    private sealed class TestAggregate : AggregateRootBase<int>
    {
        public TestAggregate()
        {
        }

        public TestAggregate(int id) => Id = id;
    }

    [Fact]
    public void Constructor_WithId_SetsId() =>
        Assert.Equal(5, new TestEntity(5).Id);

    private sealed class TestEntity(int id) : EntityBase<int>(id);

    private sealed class TestEvent(string eventId) : EventBase(eventId);
}
