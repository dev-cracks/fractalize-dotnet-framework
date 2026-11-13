// ValueObjectLegacyBase is obsolete on purpose; these tests pin its behaviour until it is removed.
#pragma warning disable CS0618

namespace Efc.Fractalize.Ddd.Tests;

public class ValueObjectLegacyBaseTest
{
    [Fact]
    public void CompareTo_WithTheSameValues_ReturnsZero()
    {
        var first = new MoneyStub { Amount = 25, Currency = "USD" };
        var second = new MoneyStub { Amount = 25, Currency = "USD" };

        Assert.Equal(0, first.CompareTo(second));
        Assert.Equal(0, first.CompareTo((object)second));
    }

    [Fact]
    public void CompareTo_WithDifferentValues_FollowsTheComponentOrder()
    {
        var thirty = new MoneyStub { Amount = 30, Currency = "USD" };
        var twentyFive = new MoneyStub { Amount = 25, Currency = "USD" };

        Assert.True(thirty.CompareTo(twentyFive) > 0);
        Assert.True(twentyFive.CompareTo(thirty) < 0);
    }

    // Characterization: contrary to its documentation, comparing with null throws.
    [Fact]
    public void CompareTo_WithNull_ThrowsNullReferenceException()
    {
        var money = new MoneyStub { Amount = 25, Currency = "USD" };

        Assert.Throws<NullReferenceException>(() => money.CompareTo(null));
    }

    [Fact]
    public void CompareTo_WithAnotherType_ThrowsInvalidCastException()
    {
        var money = new MoneyStub { Amount = 25, Currency = "USD" };

        Assert.Throws<InvalidCastException>(() => money.CompareTo("25 USD"));
    }

    [Fact]
    public void AreEqual_WithNulls_OrdersNullFirst()
    {
        var money = new MoneyStub { Amount = 25, Currency = "USD" };

        Assert.Equal(0, money.AreEqual(null!, null!));
        Assert.Equal(-1, money.AreEqual(null!, 1));
        Assert.Equal(1, money.AreEqual(1, null!));
    }

    [Fact]
    public void AreEqual_WithNonComparableValues_UsesEquals()
    {
        var money = new MoneyStub { Amount = 25, Currency = "USD" };
        var component = new object();

        Assert.Equal(0, money.AreEqual(component, component));
        Assert.Equal(-1, money.AreEqual(component, new object()));
    }

    private sealed class MoneyStub : ValueObjectLegacyBase
    {
        public required string Currency { get; init; }

        public required decimal Amount { get; init; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Currency;
            yield return Amount;
        }
    }
}
