namespace DevCracks.Fractalize.Domain.Base;

public abstract class EntityBase<TId> : IEquatable<EntityBase<TId>>
{
    public TId Id { get; set; } = default!;

    protected EntityBase()
    {
    }

    protected EntityBase(TId id) => Id = id;

    public override bool Equals(object? obj) =>
        Equals(obj as EntityBase<TId>);

    public bool Equals(EntityBase<TId>? other) =>
        other != null && EqualityComparer<TId>.Default.Equals(Id, other.Id);

    public override int GetHashCode() =>
        HashCode.Combine(Id);
}
