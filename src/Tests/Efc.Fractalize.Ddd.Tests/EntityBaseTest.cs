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
        EntityBase<int> first = new TestEntity { Id = 1 };
        EntityBase<int> second = new TestEntity { Id = 1 };

        Assert.True(first.Equals(second));
        Assert.True(first.Equals((object)second));
        Assert.True(first == second);
        Assert.False(first != second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Equals_WithADifferentId_ReturnsFalse()
    {
        EntityBase<int> first = new TestEntity { Id = 1 };
        EntityBase<int> second = new TestEntity { Id = 2 };

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
        EntityBase<int> entity = new TestEntity { Id = 1 };

        Assert.False(entity.Equals(null));
        Assert.False(entity.Equals((object?)null));
        Assert.False(entity == null);
        Assert.False(null == entity);
        Assert.True((EntityBase<int>?)null == null);
    }

    [Fact]
    public void Equals_WithTwoTransientEntities_ReturnsFalse()
    {
        EntityBase<int> first = new TestEntity { Id = 0 };
        EntityBase<int> second = new TestEntity { Id = 0 };

        Assert.False(first.Equals(second));
        Assert.False(first == second);
    }

    [Fact]
    public void Equals_WithATransientAndAPersistedEntity_ReturnsFalse()
    {
        EntityBase<int> transient = new TestEntity { Id = 0 };
        EntityBase<int> persisted = new TestEntity { Id = 1 };

        Assert.False(transient.Equals(persisted));
        Assert.False(persisted.Equals(transient));
    }

    [Fact]
    public void Equals_WithTheSameTransientInstance_ReturnsTrue()
    {
        EntityBase<int> transient = new TestEntity { Id = 0 };

        Assert.True(transient.Equals(transient));
        Assert.Equal(RuntimeHelpers.GetHashCode(transient), transient.GetHashCode());
    }

    [Fact]
    public void Equals_WithNullReferenceIds_TreatsThemAsTransient()
    {
        EntityBase<string> first = new KeyedEntity { Id = null! };
        EntityBase<string> second = new KeyedEntity { Id = null! };

        Assert.False(first.Equals(second));
        Assert.True((EntityBase<string>)new KeyedEntity { Id = "a" } == new KeyedEntity { Id = "a" });
    }

    [Fact]
    public void Entities_WithTheSameIdentity_AreDeduplicatedInASet()
    {
        HashSet<EntityBase<int>> set = [new TestEntity { Id = 1 }, new TestEntity { Id = 1 }, new TestEntity { Id = 2 }];

        Assert.Equal(2, set.Count);
    }

    [Fact]
    public void Equals_WithAnObjectThatIsNotAnEntity_ReturnsFalse()
    {
        EntityBase<int> entity = new TestEntity { Id = 1 };
        object notAnEntity = new();

        Assert.False(entity.Equals(notAnEntity));
    }

    [Fact]
    public void Equals_AcrossATwoLevelHierarchy_ComparesTheConcreteType()
    {
        EntityBase<int> parent = new ParentEntity { Id = 1 };
        EntityBase<int> child = new ChildEntity { Id = 1 };
        EntityBase<int> otherChild = new ChildEntity { Id = 1 };

        Assert.False(parent.Equals(child));
        Assert.False(child.Equals(parent));
        Assert.False(parent.Equals((object)child));
        Assert.True(child.Equals(otherChild));
        Assert.True(child.Equals((object)otherChild));
        Assert.Equal(child.GetHashCode(), otherChild.GetHashCode());
    }

    [Fact]
    public void Equals_WithAProxyThatReportsTheEntityType_ReturnsTrue()
    {
        EntityBase<int> entity = new ParentEntity { Id = 1 };
        EntityBase<int> proxy = new ParentEntityProxy { Id = 1 };

        Assert.True(entity.Equals(proxy));
        Assert.True(proxy.Equals(entity));
        Assert.True(entity.Equals((object)proxy));
        Assert.True(entity == proxy);
        Assert.Equal(entity.GetHashCode(), proxy.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ChangesWhenATransientEntityIsReplacedByItsPersistedIdentity()
    {
        EntityBase<int> transient = new TestEntity { Id = 0 };
        EntityBase<int> persisted = new TestEntity { Id = 1 };
        HashSet<EntityBase<int>> set = [transient];

        Assert.Contains(transient, set);
        Assert.DoesNotContain(persisted, set);
    }

    private sealed class TestEntity : EntityBase<int>;

    private class ParentEntity : EntityBase<int>;

    private sealed class ChildEntity : ParentEntity;

    // Simulates an ORM proxy: a runtime subclass that reports the type of the entity it stands for.
    private sealed class ParentEntityProxy : ParentEntity
    {
        protected override Type GetUnproxiedType() => typeof(ParentEntity);
    }

    private sealed class OtherEntity : EntityBase<int>;

    private sealed class KeyedEntity : EntityBase<string>;

    private sealed class ConstructedEntity : EntityBase<int>
    {
        [SetsRequiredMembers]
        public ConstructedEntity(int id) => Id = id;
    }
}
