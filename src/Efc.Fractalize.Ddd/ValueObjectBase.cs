namespace Efc.Fractalize.Ddd;

/// <summary>
/// Base record for value objects: immutable domain objects compared by their values. Record equality
/// compares every component, so use records or value types as components; a class component is compared
/// by reference.
/// </summary>
public abstract record ValueObjectBase;
