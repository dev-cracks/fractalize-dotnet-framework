namespace Efc.Fractalize.Ddd.Tests;

internal sealed class TestDomainEvent : DomainEventBase
{
    public override required string Name { get; set; }

    public override required DateTimeOffset Date { get; set; }
}
