using System.Runtime.CompilerServices;

namespace Efc.Fractalize.Ddd;

/// <summary>
/// Base class for entities: domain objects defined by their identity rather than by their attributes.
/// </summary>
/// <remarks>
/// <para>
/// The <see cref="Id"/> is set once, when the entity is created (object initializer or constructor of the
/// derived type), and cannot change afterwards.
/// </para>
/// <para>
/// Two entities are equal when they are the same instance, or when they have the same concrete type and
/// the same, non-default <see cref="Id"/>. A transient entity (its <see cref="Id"/> is still the default
/// value of <typeparamref name="TId"/>) is only equal to itself.
/// </para>
/// <para>
/// The type is taken from <see cref="GetUnproxiedType"/>, so that an ORM proxy (a runtime subclass) of an
/// entity can override it and still be equal to the entity it stands for.
/// </para>
/// <para>
/// The hash code of a transient entity is its reference and the hash code of a persisted one is based on its
/// <see cref="Id"/>. If the database generates the <see cref="Id"/> after a transient entity was added to a
/// hash-based collection (<see cref="HashSet{T}"/>, a dictionary key), the hash code changes and the
/// collection can no longer find it: add entities to such collections once their <see cref="Id"/> is set.
/// </para>
/// </remarks>
/// <typeparam name="TId">Type of the identifier.</typeparam>
public abstract class EntityBase<TId> : IEquatable<EntityBase<TId>>
{
    /// <summary>
    /// Identifier of the entity. It can only be set while the entity is being created.
    /// </summary>
    public required TId Id { get; init; }

    private bool IsTransient => Id is null || EqualityComparer<TId>.Default.Equals(Id, default!);

    /// <summary>
    /// Determines whether two entities have the same identity.
    /// </summary>
    /// <param name="left">The first entity.</param>
    /// <param name="right">The second entity.</param>
    /// <returns><see langword="true"/> when both are <see langword="null"/> or have the same identity.</returns>
    public static bool operator ==(EntityBase<TId>? left, EntityBase<TId>? right) =>
        left is null ? right is null : left.Equals(right);

    /// <summary>
    /// Determines whether two entities have different identities.
    /// </summary>
    /// <param name="left">The first entity.</param>
    /// <param name="right">The second entity.</param>
    /// <returns><see langword="true"/> when the entities do not have the same identity.</returns>
    public static bool operator !=(EntityBase<TId>? left, EntityBase<TId>? right) =>
        !(left == right);

    /// <inheritdoc />
    public bool Equals(EntityBase<TId>? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (GetUnproxiedType() != other.GetUnproxiedType() || IsTransient || other.IsTransient)
        {
            return false;
        }

        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Only another <see cref="EntityBase{TId}"/> can be equal; the type comparison itself is done by
    /// <see cref="Equals(EntityBase{TId})"/> through <see cref="GetUnproxiedType"/>.
    /// </remarks>
    public override bool Equals(object? obj) =>
        obj is not null
        && typeof(EntityBase<TId>).IsAssignableFrom(obj.GetType())
        && Equals((EntityBase<TId>)obj);

    /// <inheritdoc />
    /// <remarks>
    /// Based on <see cref="GetUnproxiedType"/> and the <see cref="Id"/>; a transient entity uses its reference
    /// instead, so the hash code changes when a generated <see cref="Id"/> is assigned (see the class remarks).
    /// </remarks>
    public override int GetHashCode() =>
        IsTransient ? RuntimeHelpers.GetHashCode(this) : HashCode.Combine(GetUnproxiedType(), Id);

    /// <summary>
    /// Type used to compare identities. Returns the runtime type; an ORM proxy overrides it to return the type
    /// of the entity it stands for.
    /// </summary>
    /// <returns>The type that defines the identity of the entity.</returns>
    protected virtual Type GetUnproxiedType() => GetType();
}
