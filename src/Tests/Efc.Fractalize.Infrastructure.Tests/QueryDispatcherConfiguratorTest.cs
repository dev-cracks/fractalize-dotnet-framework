using System.Reflection;
using Efc.Fractalize.Domain.Queries;
using Efc.Fractalize.Infrastructure.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Efc.Fractalize.Infrastructure.Tests;

public class QueryDispatcherConfiguratorTest
{
    private static readonly ServiceProviderOptions Validated = new() { ValidateOnBuild = true, ValidateScopes = true };

    [Fact]
    public async Task AddQueryDispatcher_WithHandlerAssembly_BuildsAndDispatches()
    {
        var services = new ServiceCollection();

        var result = services.AddQueryDispatcher(typeof(SampleHandler).Assembly);
        using var provider = services.BuildServiceProvider(Validated);

        Assert.Same(services, result);
        var dispatcher = provider.GetRequiredService<IQueryDispatcher<SampleQuery, string>>();
        Assert.IsType<QueryDispatcher<SampleQuery, string>>(dispatcher);
        Assert.Equal("Sample Result", await dispatcher.DispatchAsync(new SampleQuery()));
    }

    [Fact]
    public void AddQueryDispatcher_WithoutAssemblies_ScansApplicationDependencies()
    {
        var services = new ServiceCollection();

        services.AddQueryDispatcher();
        using var provider = services.BuildServiceProvider(Validated);

        Assert.IsType<SampleHandler>(provider.GetRequiredService<IQueryHandler<SampleQuery, string>>());
    }

    [Fact]
    public void AddQueryDispatcher_CalledTwice_RegistersOnce()
    {
        var assembly = typeof(SampleHandler).Assembly;
        var services = new ServiceCollection();

        services.AddQueryDispatcher(assembly).AddQueryDispatcher(assembly).AddQueryDispatcher<SampleQuery, string>();

        Assert.Single(services, d => d.ServiceType == typeof(IQueryDispatcher<,>));
        Assert.Single(services, d => d.ServiceType == typeof(IQueryHandler<SampleQuery, string>));
        Assert.Single(services, d => d.ServiceType == typeof(IQueryDispatcher<SampleQuery, string>));
    }

    [Fact]
    public void AddQueryDispatcher_KeepsImplementationsRegisteredByTheHost()
    {
        var services = new ServiceCollection();
        services.AddTransient<IQueryHandler<SampleQuery, string>, ReplacementHandler>();
        services.AddTransient<IQueryDispatcher<SampleQuery, string>, ReplacementDispatcher>();

        services.AddQueryDispatcher(typeof(SampleHandler).Assembly).AddQueryDispatcher<SampleQuery, string>();
        using var provider = services.BuildServiceProvider(Validated);

        Assert.IsType<ReplacementHandler>(provider.GetRequiredService<IQueryHandler<SampleQuery, string>>());
        Assert.IsType<ReplacementDispatcher>(provider.GetRequiredService<IQueryDispatcher<SampleQuery, string>>());
    }

    [Fact]
    public void AddQueryDispatcherOfQuery_BuildsAndResolves()
    {
        var services = new ServiceCollection();

        var result = services.AddQueryDispatcher<SampleQuery, string>();
        using var provider = services.BuildServiceProvider(Validated);

        Assert.Same(services, result);
        Assert.IsType<QueryDispatcher<SampleQuery, string>>(
            provider.GetRequiredService<IQueryDispatcher<SampleQuery, string>>());
    }

    [Fact]
    public void AddQueryDispatcher_WithNullArguments_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ((IServiceCollection)null!).AddQueryDispatcher());
        Assert.Throws<ArgumentNullException>(() => new ServiceCollection().AddQueryDispatcher((Assembly[])null!));
        Assert.Throws<ArgumentNullException>(
            () => ((IServiceCollection)null!).AddQueryDispatcher<SampleQuery, string>());
    }

    private sealed class ReplacementHandler : IQueryHandler<SampleQuery, string>
    {
        public Task<string> HandleAsync(SampleQuery query, CancellationToken cancellationToken = default) =>
            Task.FromResult("Replacement");
    }

    private sealed class ReplacementDispatcher : IQueryDispatcher<SampleQuery, string>
    {
        public Task<string> DispatchAsync(SampleQuery query, CancellationToken cancellationToken = default) =>
            Task.FromResult("Replacement");
    }
}
