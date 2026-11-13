using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Efc.Fractalize.Ddd.Tests;

public class EntityBaseTest
{
    // Regression test (ADR 0087 n.º 3): the Id used to have a public setter. It is init-only now, so
    // `entity.Id = 2;` no longer compiles after creation.
    [Fact]
    public void Id_IsInitOnly()
    {
        var setter = typeof(EntityBase<int>).GetProperty(nameof(EntityBase<>.Id))!.SetMethod!;

        Assert.Contains(typeof(IsExternalInit), setter.ReturnParameter.GetRequiredCustomModifiers());
    }

    [Fact]
    public void Id_CanBeSetByTheConstructorOfTheDerivedType()
    {
        var entity = new ConstructedEntity(5);

        Assert.Equal(5, entity.Id);
    }

    // Regression test (ADR 0087 n.º 3): entities used to have reference equality only.
    [Fact]
    public void Equals_WithTheSameTypeAndId_ReturnsTrue()
    {
        var first = new TestEntity { Id = 1 };
        var second = new TestEntity { Id = 1 };

        Assert.True(first.Equals(second));
        Assert.True(first.Equals((object)second));
        Assert.True(first == second);
        Assert.False(first != second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Equals_WithADifferentId_ReturnsFalse()
    {
        var first = new TestEntity { Id = 1 };
        var second = new TestEntity { Id = 2 };

        Assert.False(first.Equals(second));
        Assert.False(first == second);
        Assert.True(first != second);
    }

    [Fact]
    public void Equals_WithTheSameIdAndAnotherType_ReturnsFalse()
    {
        EntityBase<int> entity = new TestEntity { Id = 1 };
        EntityBase<int> other = new OtherEntity { Id = 1 };

        Assert.False(entity.Equals(other));
        Assert.False(entity == other);
    }

    [Fact]
    public void Equals_WithNullOrAnotherObject_ReturnsFalse()
    {
        var entity = new TestEntity { Id = 1 };

        Assert.False(entity.Equals(null));
        Assert.False(entity.Equals((object?)null));
        Assert.False(entity.Equals("1"));
        Assert.False(entity == null);
        Assert.False(null == entity);
        Assert.True((TestEntity?)null == null);
    }

    [Fact]
    public void Equals_WithTwoTransientEntities_ReturnsFalse()
    {
        var first = new TestEntity { Id = 0 };
        var second = new TestEntity { Id = 0 };

        Assert.False(first.Equals(second));
        Assert.False(first == second);
    }

    [Fact]
    public void Equals_WithATransientAndAPersistedEntity_ReturnsFalse()
    {
        var transient = new TestEntity { Id = 0 };
        var persisted = new TestEntity { Id = 1 };

        Assert.False(transient.Equals(persisted));
        Assert.False(persisted.Equals(transient));
    }

    [Fact]
    public void Equals_WithTheSameTransientInstance_ReturnsTrue()
    {
        var transient = new TestEntity { Id = 0 };

        Assert.True(transient.Equals(transient));
        Assert.Equal(RuntimeHelpers.GetHashCode(transient), transient.GetHashCode());
    }

    [Fact]
    public void Equals_WithNullReferenceIds_TreatsThemAsTransient()
    {
        var first = new KeyedEntity { Id = null! };
        var second = new KeyedEntity { Id = null! };

        Assert.False(first.Equals(second));
        Assert.True(new KeyedEntity { Id = "a" } == new KeyedEntity { Id = "a" });
    }

    [Fact]
    public void Entities_WithTheSameIdentity_AreDeduplicatedInASet()
    {
        HashSet<TestEntity> set = [new() { Id = 1 }, new() { Id = 1 }, new() { Id = 2 }];

        Assert.Equal(2, set.Count);
    }

    private sealed class TestEntity : EntityBase<int>;

    private sealed class OtherEntity : EntityBase<int>;

    private sealed class KeyedEntity : EntityBase<string>;

    private sealed class ConstructedEntity : EntityBase<int>
    {
        [SetsRequiredMembers]
        public ConstructedEntity(int id) => Id = id;
    }
}
