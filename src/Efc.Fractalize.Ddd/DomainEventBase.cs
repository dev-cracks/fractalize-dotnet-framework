namespace Efc.Fractalize.Ddd;

/// <summary>
/// TODO
/// </summary>
public abstract class DomainEventBase
{
    public abstract required string Name { get; set; }

    /// <summary>
    /// DateTimeOffset ISO 8601
    /// </summary>
    public abstract required DateTimeOffset Date { get; set; }
}
