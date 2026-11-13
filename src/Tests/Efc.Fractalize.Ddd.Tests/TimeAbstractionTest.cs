using Microsoft.Extensions.Time.Testing;

namespace Efc.Fractalize.Ddd.Tests;

public class TimeAbstractionTest
{
    // Regression test (ADR 0087 n.º 3): the package used to ship its own clock abstraction,
    // IClockStamp. Time comes from System.TimeProvider instead.
    [Fact]
    public void Package_DoesNotDeclareItsOwnClockAbstraction()
    {
        var types = typeof(EntityBase<>).Assembly.GetExportedTypes();

        Assert.DoesNotContain(types, type => type.Name == "IClockStamp");
        Assert.DoesNotContain(
            types.SelectMany(type => type.GetMethods()).SelectMany(method => method.GetParameters()),
            parameter => parameter.ParameterType.Name.Contains("Clock", StringComparison.Ordinal));
    }

    [Fact]
    public void AggregateEvents_StampedFromATimeProvider_FollowItsClock()
    {
        var timeProvider = new FakeTimeProvider(TestClock.Now);
        var order = TestOrder.Place(timeProvider);

        timeProvider.Advance(TimeSpan.FromMinutes(5));
        order.Rename(timeProvider);

        Assert.Equal(
            [TestClock.Now, TestClock.Now.AddMinutes(5)],
            order.DomainEvents.Select(domainEvent => domainEvent.Date));
    }

    private sealed class TestOrder : AggregateRootBase<Guid>
    {
        public static TestOrder Place(TimeProvider timeProvider)
        {
            var order = new TestOrder { Id = Guid.NewGuid() };
            order.AddDomainEvent(new TestDomainEvent { Name = "placed", Date = timeProvider.GetUtcNow() });
            return order;
        }

        public void Rename(TimeProvider timeProvider) =>
            AddDomainEvent(new TestDomainEvent { Name = "renamed", Date = timeProvider.GetUtcNow() });
    }
}
