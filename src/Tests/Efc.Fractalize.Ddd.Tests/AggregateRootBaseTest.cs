namespace Efc.Fractalize.Ddd.Tests;

public class AggregateRootBaseTest
{
    [Fact]
    public void AddDomainEvent_WithAnEvent_ExposesItInDomainEvents()
    {
        var aggregate = new TestAggregate { Id = Guid.NewGuid() };
        var domainEvent = new TestDomainEvent { Name = "created", Date = TestClock.Now };

        aggregate.AddDomainEvent(domainEvent);

        Assert.Same(domainEvent, Assert.Single(aggregate.DomainEvents));
    }

    [Fact]
    public void DomainEvents_OnANewAggregate_IsEmpty()
    {
        var aggregate = new TestAggregate { Id = Guid.NewGuid() };

        Assert.Empty(aggregate.DomainEvents);
    }

    [Fact]
    public void AddDomainEvent_WithSeveralEvents_KeepsTheirOrder()
    {
        var aggregate = new TestAggregate { Id = Guid.NewGuid() };
        var created = new TestDomainEvent { Name = "created", Date = TestClock.Now };
        var renamed = new TestDomainEvent { Name = "renamed", Date = TestClock.Now };

        aggregate.AddDomainEvent(created);
        aggregate.AddDomainEvent(renamed);

        Assert.Equal([created, renamed], aggregate.DomainEvents);
    }

    // Regression test (ADR 0087 n.º 3): ClearDomainEvents used to add the event instead of
    // emptying the list.
    [Fact]
    public void ClearDomainEvents_WithPendingEvents_RemovesThemAll()
    {
        var aggregate = new TestAggregate { Id = Guid.NewGuid() };
        aggregate.AddDomainEvent(new TestDomainEvent { Name = "created", Date = TestClock.Now });
        aggregate.AddDomainEvent(new TestDomainEvent { Name = "renamed", Date = TestClock.Now });

        aggregate.ClearDomainEvents();

        Assert.Empty(aggregate.DomainEvents);
    }

    [Fact]
    public void ClearDomainEvents_OnANewAggregate_KeepsItEmpty()
    {
        var aggregate = new TestAggregate { Id = Guid.NewGuid() };

        aggregate.ClearDomainEvents();

        Assert.Empty(aggregate.DomainEvents);
    }

    [Fact]
    public void AddDomainEvent_AfterClearing_StartsANewList()
    {
        var aggregate = new TestAggregate { Id = Guid.NewGuid() };
        aggregate.AddDomainEvent(new TestDomainEvent { Name = "created", Date = TestClock.Now });
        aggregate.ClearDomainEvents();
        var renamed = new TestDomainEvent { Name = "renamed", Date = TestClock.Now };

        aggregate.AddDomainEvent(renamed);

        Assert.Same(renamed, Assert.Single(aggregate.DomainEvents));
    }

    private sealed class TestAggregate : AggregateRootBase<Guid>;
}
