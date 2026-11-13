using Efc.Fractalize.Domain.Queries;
using Efc.Fractalize.Infrastructure.Extensions;
using Efc.Fractalize.Infrastructure.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Efc.Fractalize.Infrastructure.Tests;

// These tests cover the 2.x registration helpers on purpose: they must keep working until they are removed.
#pragma warning disable CS0618 // Type or member is obsolete
public class ObsoleteConfigurationExtensionsTest
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

    [Fact]
    public async Task TryAddQueryDispatcher_WithHandlers_Dispatches()
    {
        var services = new ServiceCollection();
        services.TryAddQueryDispatcher<SampleQuery, string>();
        services.AddQueryHandlers();
        var dispatcher = services.BuildServiceProvider().GetRequiredService<IQueryDispatcher<SampleQuery, string>>();

        Assert.Equal("Sample Result", await dispatcher.DispatchAsync(new SampleQuery()));
    }

    [Fact]
    public void AddQueryDispatchers_ReturnsTheSameServiceCollection()
    {
        var services = new ServiceCollection();

        Assert.Same(services, services.AddQueryDispatchers());
    }

    [Fact]
    public void ConfigureQueryHandler_RegistersTheQueryDispatcher()
    {
        var services = new ServiceCollection();

        var result = services.ConfigureQueryHandler<SampleQuery, string>();

        Assert.Same(services, result);
        Assert.Contains(services, d => d.ServiceType == typeof(IQuery<string>));
        Assert.Contains(services, d => d.ServiceType == typeof(IQueryDispatcher<SampleQuery, string>));
    }

    public sealed class SampleOptions
    {
        public string Name { get; set; } = string.Empty;

        public int Retries { get; set; }
    }
}
#pragma warning restore CS0618
