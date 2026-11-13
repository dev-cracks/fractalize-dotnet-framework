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
        string[] buildingBlocks =
            ["Entity", "AggregateRoot", "DomainEvent", "EventBase", "ValueObject", "Repository", "UnitOfWork"];
        var duplicates = Assembly.Load(assemblyName).GetExportedTypes()
            .Where(type => assemblyName != "Efc.Fractalize.Ddd"
                && buildingBlocks.Any(block => type.Name.Contains(block, StringComparison.Ordinal)))
            .Select(type => type.FullName);

        Assert.Empty(duplicates);
    }

    // The other assemblies build on Efc.Fractalize.Ddd instead of extending its building blocks publicly.
    [Theory]
    [MemberData(nameof(PublicAssemblies))]
    public void PublicTypes_OutsideDdd_DoNotDeriveFromDddTypes(string assemblyName)
    {
        var ddd = typeof(Efc.Fractalize.Ddd.EntityBase<>).Assembly;
        var derived = Assembly.Load(assemblyName).GetExportedTypes()
            .Where(type => type.Assembly != ddd && BaseTypesAndInterfaces(type).Any(baseType => baseType.Assembly == ddd))
            .Select(type => type.FullName);

        Assert.Empty(derived);
    }

    // The common base stays at the bottom: it depends on no other assembly of the framework.
    [Fact]
    public void Ddd_DoesNotReferenceOtherFrameworkAssemblies()
    {
        var references = typeof(Efc.Fractalize.Ddd.EntityBase<>).Assembly.GetReferencedAssemblies()
            .Where(reference => reference.Name!.StartsWith("Efc.Fractalize.", StringComparison.Ordinal))
            .Select(reference => reference.Name);

        Assert.Empty(references);
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

    private static IEnumerable<Type> BaseTypesAndInterfaces(Type type)
    {
        for (var baseType = type.BaseType; baseType is not null; baseType = baseType.BaseType)
        {
            yield return baseType.IsGenericType ? baseType.GetGenericTypeDefinition() : baseType;
        }

        foreach (var implemented in type.GetInterfaces())
        {
            yield return implemented.IsGenericType ? implemented.GetGenericTypeDefinition() : implemented;
        }
    }
}
