using System.Reflection;

namespace Efc.Fractalize.Architecture.Tests;

/// <summary>
/// The framework is public: business concepts of Fractalize (accounts, subscriptions, tenants, auditing,
/// feature flags, entitlements...) belong to private packages, never to these assemblies (ADR 0094 n.º 3).
/// </summary>
public class PublicBoundaryTest
{
    private static readonly string[] BusinessTerms =
    [
        "Account", "Subscription", "Tenant", "MultiTenancy", "Audit", "FeatureFlag", "Suppression",
        "Entitlement", "Product", "Fractal", "Billing", "Price",
    ];

    private static readonly string[] PublicAssemblyNames =
    [
        "Efc.Fractalize.Domain",
        "Efc.Fractalize.Ddd",
        "Efc.Fractalize.Application",
        "Efc.Fractalize.Infrastructure",
    ];

    public static TheoryData<string> PublicAssemblies() => [.. PublicAssemblyNames];

    [Theory]
    [MemberData(nameof(PublicAssemblies))]
    public void PublicTypes_DoNotNameBusinessConcepts(string assemblyName)
    {
        var offending = Assembly.Load(assemblyName).GetExportedTypes()
            .Where(type => BusinessTerms.Any(term => type.Name.Contains(term, StringComparison.OrdinalIgnoreCase)))
            .Select(type => type.FullName);

        Assert.Empty(offending);
    }

    [Theory]
    [MemberData(nameof(PublicAssemblies))]
    public void PublicTypes_LiveUnderTheAssemblyNamespace(string assemblyName)
    {
        var outside = Assembly.Load(assemblyName).GetExportedTypes()
            .Where(type => type.Namespace is null
                || (type.Namespace != assemblyName && !type.Namespace.StartsWith(assemblyName + ".", StringComparison.Ordinal)))
            .Select(type => type.FullName);

        Assert.Empty(outside);
    }

    // Efc.Fractalize.Ddd is the common DDD base: no other assembly declares its own building blocks.
    [Theory]
    [MemberData(nameof(PublicAssemblies))]
    public void DddBuildingBlocks_AreDeclaredOnlyInEfcFractalizeDdd(string assemblyName)
    {
        string[] buildingBlocks = ["EntityBase", "AggregateRootBase", "DomainEvent", "EventBase", "ValueObject"];
        var duplicates = Assembly.Load(assemblyName).GetExportedTypes()
            .Where(type => assemblyName != "Efc.Fractalize.Ddd"
                && buildingBlocks.Any(block => type.Name.Contains(block, StringComparison.Ordinal)))
            .Select(type => type.FullName);

        Assert.Empty(duplicates);
    }

    [Fact]
    public void PublicAssemblies_ListEveryLibraryOfTheRepository()
    {
        var src = new DirectoryInfo(AppContext.BaseDirectory);
        while (src is not null && !File.Exists(Path.Join(src.FullName, "Efc.Fractalize.Framework.sln")))
        {
            src = src.Parent;
        }

        Assert.NotNull(src);
        var libraries = src.GetDirectories("Efc.Fractalize.*")
            .Where(directory => File.Exists(Path.Join(directory.FullName, directory.Name + ".csproj")))
            .Select(directory => directory.Name)
            .Order();

        Assert.Equal(PublicAssemblyNames.Order(), libraries);
    }
}
