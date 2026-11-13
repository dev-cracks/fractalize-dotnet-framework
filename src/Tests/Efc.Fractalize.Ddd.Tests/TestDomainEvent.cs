namespace Efc.Fractalize.Ddd.Tests;

internal sealed class TestDomainEvent : DomainEventBase
{
    public override required string Name { get; init; }

    public override required DateTimeOffset Date { get; init; }
}
