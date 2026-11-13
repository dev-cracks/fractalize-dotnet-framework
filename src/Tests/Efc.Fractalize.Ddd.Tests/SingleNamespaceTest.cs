namespace Efc.Fractalize.Ddd.Tests;

public class SingleNamespaceTest
{
    // ADR 0087 n.º 3: the DDD core keeps a single namespace.
    [Fact]
    public void PublicTypes_LiveInASingleNamespace()
    {
        var namespaces = typeof(EntityBase<>).Assembly.GetExportedTypes().Select(type => type.Namespace).Distinct();

        Assert.Equal(["Efc.Fractalize.Ddd"], namespaces);
    }
}
