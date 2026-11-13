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
/// Overlaps with <c>Efc.Fractalize.Domain.Entities.EntityBase&lt;TId&gt;</c>
/// in the <c>Efc.Fractalize.Domain</c> package. Both are kept until the maintainers decide which one
/// survives; do not mix them in the same model.
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

        if (GetType() != other.GetType() || IsTransient || other.IsTransient)
        {
            return false;
        }

        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) =>
        Equals(obj as EntityBase<TId>);

    /// <inheritdoc />
    /// <remarks>
    /// Based on the concrete type and the <see cref="Id"/>, which cannot change; a transient entity uses its
    /// reference instead.
    /// </remarks>
    public override int GetHashCode() =>
        IsTransient ? RuntimeHelpers.GetHashCode(this) : HashCode.Combine(GetType(), Id);
}
