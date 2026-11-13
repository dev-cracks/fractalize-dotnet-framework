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

    // Characterization of the inherited bug (ADR 0087 n.º 3): ClearDomainEvents adds the event
    // instead of emptying the list. A later commit fixes it on purpose.
    [Fact]
    public void ClearDomainEvents_WithAnEvent_AddsItInsteadOfClearing()
    {
        var aggregate = new TestAggregate { Id = Guid.NewGuid() };
        var created = new TestDomainEvent { Name = "created", Date = TestClock.Now };
        aggregate.AddDomainEvent(created);

        aggregate.ClearDomainEvents(created);

        Assert.Equal([created, created], aggregate.DomainEvents);
    }

    private sealed class TestAggregate : AggregateRootBase<Guid>;
}
