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
    public void DomainEventDate_IsStampedFromATimeProvider()
    {
        TimeProvider timeProvider = new FixedTimeProvider(TestClock.Now);

        var domainEvent = new TestDomainEvent { Name = "created", Date = timeProvider.GetUtcNow() };

        Assert.Equal(TestClock.Now, domainEvent.Date);
    }

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}
