using DevCracks.Fractalize.Domain.Events;

namespace DevCracks.Fractalize.Domain.Entities;

/// <summary>
/// Base class for events in the domain.
/// This class is designed to be used with an enum type that represents the event type.
/// It provides properties for event metadata such as creation time, event ID, correlation ID, and causation ID.
/// It also includes constructors for initializing these properties.
/// The class overrides the ToString method to provide a string representation of the event,
/// and the Equals method to compare events based on their properties.
/// </summary>
/// <typeparam name="TId"></typeparam>
public abstract class EntityBase<TId> : IEquatable<EntityBase<TId>>
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// This property is of type TId, which allows for flexibility in the type of identifier used.
    /// It is initialized to the default value of TId, which is typically null for reference types or zero for value types.
    /// The Id property is protected, allowing it to be set only within derived classes.
    /// It is also marked as virtual to allow for overriding in derived classes if needed.
    /// The Id property is essential for identifying the entity uniquely within the domain.
    /// It is used in comparisons, hash code generation, and serialization.
    /// The entity's ID is crucial for tracking its state and behavior within the domain model.
    /// </summary>
    public TId Id { get; protected set; } = default!;
    /// <summary>
    /// The list of events that have occurred on this aggregate root.
    /// This list is used to track changes and can be used for event sourcing or other purposes.
    /// </summary>
    protected List<IDomainEvent> _events = [];

    /// <summary>
    /// The list of events that have occurred on this aggregate root.
    /// </summary>
    public IReadOnlyList<IDomainEvent> Events => _events.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityBase{TId}"/> class.
    /// </summary>
    protected EntityBase() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityBase{TId}"/> class with a specified ID.
    /// This constructor is used to create an entity with a specific identifier.
    /// </summary>
    /// <param name="id"></param>
    protected EntityBase(TId id) => Id = id;

    /// <summary>
    /// Returns a string representation of the entity.
    /// This method overrides the default ToString method to provide a meaningful representation of the entity,
    /// typically including the entity type and its ID.
    /// </summary>
    public override string ToString() =>
        $"{GetType().Name} [Id={Id}]";

    /// <summary>
    /// Determines whether the specified object is equal to the current entity.
    /// This method overrides the default Equals method to compare the current entity with another object.
    /// It checks if the other object is of the same type and compares their IDs for equality.
    /// </summary>
    public override bool Equals(object? obj) =>
        Equals(obj as EntityBase<TId>);

    /// <summary>
    /// Determines whether the specified entity is equal to the current entity.
    /// This method implements the IEquatable interface to provide a strongly-typed comparison.
    /// It checks if the other entity is not null and compares their IDs for equality.
    /// </summary>
    public bool Equals(EntityBase<TId>? other) =>
        other != null && EqualityComparer<TId>.Default.Equals(Id, other.Id);

    /// <summary>
    /// Returns a hash code for the current entity.
    /// This method overrides the default GetHashCode method to provide a hash code based on the entity's ID.
    /// It is used in hash-based collections to efficiently locate the entity.
    /// </summary>
    public override int GetHashCode() =>
        HashCode.Combine(Id);
}
