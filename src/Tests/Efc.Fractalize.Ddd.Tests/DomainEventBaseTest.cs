using System.Runtime.CompilerServices;

namespace Efc.Fractalize.Ddd.Tests;

public class DomainEventBaseTest
{
    [Fact]
    public void NameAndDate_AreSetWhenTheEventIsCreated()
    {
        var domainEvent = new TestDomainEvent { Name = "created", Date = TestClock.Now };

        Assert.Equal("created", domainEvent.Name);
        Assert.Equal(TestClock.Now, domainEvent.Date);
    }

    // Name and Date used to have public setters: an event is a fact and cannot change once raised.
    [Theory]
    [InlineData(nameof(DomainEventBase.Name))]
    [InlineData(nameof(DomainEventBase.Date))]
    public void NameAndDate_AreAbstractAndInitOnly(string propertyName)
    {
        var property = typeof(DomainEventBase).GetProperty(propertyName)!;

        Assert.True(property.GetMethod!.IsAbstract);
        Assert.Contains(typeof(IsExternalInit), property.SetMethod!.ReturnParameter.GetRequiredCustomModifiers());
    }
}
