namespace Efc.Fractalize.Ddd.Tests;

public class DomainEventBaseTest
{
    [Fact]
    public void NameAndDate_AreSetByTheDerivedEventAndCanChange()
    {
        var domainEvent = new TestDomainEvent { Name = "created", Date = TestClock.Now };

        domainEvent.Name = "renamed";
        domainEvent.Date = TestClock.Now.AddMinutes(1);

        Assert.Equal("renamed", domainEvent.Name);
        Assert.Equal(TestClock.Now.AddMinutes(1), domainEvent.Date);
    }

    [Fact]
    public void NameAndDate_AreAbstract()
    {
        Assert.True(typeof(DomainEventBase).GetProperty(nameof(DomainEventBase.Name))!.GetMethod!.IsAbstract);
        Assert.True(typeof(DomainEventBase).GetProperty(nameof(DomainEventBase.Date))!.GetMethod!.IsAbstract);
    }
}
