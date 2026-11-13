namespace Efc.Fractalize.Ddd.Tests;

public class PublicBoundaryTest
{
    // The framework is public: business concepts (accounts, subscriptions, tenants, auditing, products,
    // entitlements...) belong to private packages, never to this one.
    private static readonly string[] BusinessTerms =
    [
        "Account", "Subscription", "Tenant", "Audit", "Product", "Fractal", "Entitlement", "Billing", "Price",
    ];

    [Fact]
    public void PublicTypes_DoNotNameBusinessConcepts()
    {
        var offending = typeof(EntityBase<>).Assembly.GetExportedTypes()
            .Where(type => BusinessTerms.Any(term => type.Name.Contains(term, StringComparison.OrdinalIgnoreCase)))
            .Select(type => type.FullName);

        Assert.Empty(offending);
    }

    [Fact]
    public void PublicTypes_LiveInASingleNamespace()
    {
        var namespaces = typeof(EntityBase<>).Assembly.GetExportedTypes().Select(type => type.Namespace).Distinct();

        Assert.Equal(["Efc.Fractalize.Ddd"], namespaces);
    }
}
