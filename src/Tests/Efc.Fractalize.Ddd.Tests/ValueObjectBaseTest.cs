namespace Efc.Fractalize.Ddd.Tests;

public class ValueObjectBaseTest
{
    [Fact]
    public void Equality_WithDifferentInstancesOfAClassComponent_IsFalse()
    {
        var first = new AddressStub("Main Street", 3, new ClassStub { Street = "Main Street", Number = 3 });
        var second = new AddressStub("Main Street", 3, new ClassStub { Street = "Main Street", Number = 3 });

        Assert.False(first == second);
    }

    [Fact]
    public void Equality_WithTheSameInstanceOfAClassComponent_IsTrue()
    {
        var component = new ClassStub { Street = "Main Street", Number = 3 };

        var first = new AddressStub("Main Street", 3, component);
        var second = new AddressStub("Main Street", 3, component);

        Assert.True(first == second);
    }

    [Fact]
    public void Equality_OfPlainClasses_IsReferenceEquality()
    {
        var first = new ClassStub { Street = "Main Street", Number = 3 };
        var second = new ClassStub { Street = "Main Street", Number = 3 };

        Assert.False(first == second);
    }

    [Fact]
    public void Equality_WithEqualRecordComponents_IsTrue()
    {
        var first = new AddressWithNameStub("Main Street", 3, new NameStub("Jane", "Doe"));
        var second = new AddressWithNameStub("Main Street", 3, new NameStub("Jane", "Doe"));

        Assert.True(first == second);
    }

    private sealed record AddressStub(string Street, int Number, ClassStub Component) : ValueObjectBase;

    private sealed record AddressWithNameStub(string Street, int Number, NameStub Name) : ValueObjectBase;

    private sealed class ClassStub
    {
        public required string Street { get; init; }

        public required int Number { get; init; }
    }

    private sealed record NameStub(string FirstName, string LastName);
}
