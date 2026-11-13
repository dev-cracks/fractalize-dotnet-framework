namespace Efc.Fractalize.Ddd.Tests;

public class EntityBaseTest
{
    // Characterization of the inherited behaviour (ADR 0087 n.º 3): the Id can be changed after
    // creation. A later commit makes it immutable on purpose.
    [Fact]
    public void Id_CanBeChangedAfterCreation()
    {
        var entity = new TestEntity { Id = 1 };

        entity.Id = 2;

        Assert.Equal(2, entity.Id);
    }

    // Characterization of the inherited behaviour (ADR 0087 n.º 3): there is no identity equality,
    // two entities with the same Id are different objects. A later commit adds identity equality.
    [Fact]
    public void Equals_WithTheSameId_UsesReferenceEquality()
    {
        var first = new TestEntity { Id = 1 };
        var second = new TestEntity { Id = 1 };

        Assert.False(first.Equals(second));
        Assert.False(first == second);
        Assert.True(first.Equals(first));
    }

    private sealed class TestEntity : EntityBase<int>;
}
