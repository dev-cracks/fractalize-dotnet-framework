namespace Efc.Fractalize.Ddd;

/// <summary>
/// Serves as the base class for value objects, providing a foundation for implementing value-based equality and
/// immutability in domain models.
/// </summary>
/// <remarks>Inherit from this class to create value objects that represent domain concepts defined by their
/// properties rather than identity. Derived classes should override equality members to ensure correct value
/// comparison. This class is intended for use in domain-driven design patterns.</remarks>
[Obsolete($"Use {nameof(ValueObjectBase)} instead")]
public abstract class ValueObjectLegacyBase : IComparable, IComparable<ValueObjectLegacyBase>
{
    /// <summary>
    /// Provides the components that are used to determine equality for the current object.
    /// </summary>
    /// <remarks>Override this method in a derived class to specify which fields or properties should be
    /// considered when evaluating equality. The returned components are compared in order to determine whether two
    /// instances are equal.</remarks>
    /// <returns>An enumerable collection of objects that represent the values used to compare this instance for equality.</returns>
    protected abstract IEnumerable<object> GetEqualityComponents();

    /// <summary>
    /// Compares two objects and returns an integer that indicates their relative order or equality.
    /// </summary>
    /// <remarks>If both parameters are null, the method returns 0. If only one parameter is null, the null
    /// value is considered less than the non-null value. If both objects implement IComparable, their CompareTo method
    /// is used for comparison. Otherwise, equality is determined using the Equals method.</remarks>
    /// <param name="objectA">The first object to compare. This parameter can be null.</param>
    /// <param name="objectB">The second object to compare. This parameter can be null.</param>
    /// <returns>A value less than zero if objectA is less than objectB; zero if objectA and objectB are equal; or a value
    /// greater than zero if objectA is greater than objectB.</returns>
    public int AreEqual(object objectA, object objectB)
    {
        if (objectA is null && objectB is null)
        {
            return 0;
        }

        if (objectA is null)
        {
            return -1;
        }

        if (objectB is null)
        {
            return 1;
        }

        if (objectA is IComparable comparableA && objectB is IComparable comparableB)
        {
            return comparableA.CompareTo(comparableB);
        }

        return objectA.Equals(objectB) ? 0 : -1;
    }

    /// <summary>
    /// Compares the current instance with another value object of the same type and returns an integer that indicates
    /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the
    /// specified object.
    /// </summary>
    /// <remarks>This method implements the <see cref="IComparable{T}"/> interface, enabling value objects to
    /// be compared and sorted. If <paramref name="other"/> is null, the current instance is considered
    /// greater.</remarks>
    /// <param name="other">The value object to compare with the current instance. This parameter can be null.</param>
    /// <returns>A value less than zero if the current instance precedes <paramref name="other"/> in the sort order; zero if they
    /// are equal; or a value greater than zero if the current instance follows <paramref name="other"/> in the sort
    /// order.</returns>
    public int CompareTo(ValueObjectLegacyBase? other) =>
        CompareTo(other as object);

    /// <summary>
    /// Compares the current value object with another object of the same type and returns an integer that indicates
    /// their relative order.
    /// </summary>
    /// <remarks>Comparison is performed by evaluating the equality components of both value objects in order.
    /// Both objects must have the same number and type of equality components for a valid comparison.</remarks>
    /// <param name="targetObject">The object to compare with the current instance. Must be of the same type as the current value object.</param>
    /// <returns>A signed integer that indicates the relative order of the objects being compared: less than zero if the current
    /// instance precedes the target object; zero if they are equal; greater than zero if the current instance follows
    /// the aceptable types are <see cref="ValueObjectLegacyBase"/>
    /// the target object.</returns>
    public int CompareTo(object? targetObject)
    {
        var targetObj = (ValueObjectLegacyBase)targetObject!;

        object[] sourceComponents = GetEqualityComponents().ToArray();
        object[] targetComponents = targetObj!.GetEqualityComponents()!.ToArray();

        for (int i = 0; i < sourceComponents.Length; i++)
        {
            int comparisionResult = AreEqual(sourceComponents[i], targetComponents[i]);
            if (comparisionResult != 0)
            {
                return comparisionResult;
            }
        }

        return 0;
    }
}

