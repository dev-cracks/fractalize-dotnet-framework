using DevCracks.Fractalize.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Dev.Cracks.Fractalize.Infrastructure.Test;

public class AddConfigurationSectionTest
{
    [Fact]
    public void AddConfigurationSection_WithExistingSection_BindsOptions()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Sample:Name"] = "fractalize", ["Sample:Retries"] = "3" })
            .Build();
        var services = new ServiceCollection();

        var result = services.AddConfigurationSection<SampleOptions>(configuration, "Sample");
        var options = services.BuildServiceProvider().GetRequiredService<IOptions<SampleOptions>>().Value;

        Assert.Same(services, result);
        Assert.Equal("fractalize", options.Name);
        Assert.Equal(3, options.Retries);
    }

    [Fact]
    public void AddConfigurationSection_WithMissingSection_RegistersNothing()
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();

        services.AddConfigurationSection<SampleOptions>(configuration, "Missing");

        Assert.Empty(services);
    }

    public sealed class SampleOptions
    {
        public string Name { get; set; } = string.Empty;

        public int Retries { get; set; }
    }
}
