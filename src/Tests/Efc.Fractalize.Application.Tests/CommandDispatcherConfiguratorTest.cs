using System.Reflection;
using Efc.Fractalize.Application.Commands;
using Efc.Fractalize.Domain.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Efc.Fractalize.Application.Tests;

public class CommandDispatcherConfiguratorTest
{
    private static readonly ServiceProviderOptions Validated = new() { ValidateOnBuild = true, ValidateScopes = true };

    [Fact]
    public async Task AddCommandDispatcher_WithHandlerAssembly_BuildsAndDispatches()
    {
        var services = new ServiceCollection();

        var result = services.AddCommandDispatcher(typeof(SampleHandler).Assembly);
        using var provider = services.BuildServiceProvider(Validated);

        Assert.Same(services, result);
        var dispatcher = provider.GetRequiredService<ICommandDispatcher<SampleCommand>>();
        Assert.IsType<CommandDispatcher<SampleCommand>>(dispatcher);
        Assert.IsType<SampleHandler>(provider.GetRequiredService<ICommandHandler<SampleCommand>>());
        await dispatcher.DispatchAsync(new SampleCommand());
    }

    [Fact]
    public void AddCommandDispatcher_WithoutAssemblies_ScansApplicationDependencies()
    {
        var services = new ServiceCollection();

        services.AddCommandDispatcher();
        using var provider = services.BuildServiceProvider(Validated);

        Assert.IsType<SampleHandler>(provider.GetRequiredService<ICommandHandler<SampleCommand>>());
    }

    [Fact]
    public void AddCommandDispatcher_CalledTwice_RegistersOnce()
    {
        var assembly = typeof(SampleHandler).Assembly;
        var services = new ServiceCollection();

        services.AddCommandDispatcher(assembly).AddCommandDispatcher(assembly).AddCommandDispatcher<SampleCommand>();

        Assert.Single(services, d => d.ServiceType == typeof(ICommandDispatcher<>));
        Assert.Single(services, d => d.ServiceType == typeof(ICommandHandler<SampleCommand>));
        Assert.Single(services, d => d.ServiceType == typeof(ICommandDispatcher<SampleCommand>));
    }

    [Fact]
    public void AddCommandDispatcher_KeepsImplementationsRegisteredByTheHost()
    {
        var services = new ServiceCollection();
        services.AddTransient<ICommandHandler<SampleCommand>, ReplacementHandler>();
        services.AddTransient<ICommandDispatcher<SampleCommand>, ReplacementDispatcher>();

        services.AddCommandDispatcher(typeof(SampleHandler).Assembly).AddCommandDispatcher<SampleCommand>();
        using var provider = services.BuildServiceProvider(Validated);

        Assert.IsType<ReplacementHandler>(provider.GetRequiredService<ICommandHandler<SampleCommand>>());
        Assert.IsType<ReplacementDispatcher>(provider.GetRequiredService<ICommandDispatcher<SampleCommand>>());
    }

    [Fact]
    public void AddCommandDispatcherOfCommand_BuildsAndResolves()
    {
        var services = new ServiceCollection();

        var result = services.AddCommandDispatcher<SampleCommand>();
        using var provider = services.BuildServiceProvider(Validated);

        Assert.Same(services, result);
        Assert.IsType<CommandDispatcher<SampleCommand>>(provider.GetRequiredService<ICommandDispatcher<SampleCommand>>());
    }

    [Fact]
    public void AddCommandDispatcher_WithNullArguments_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ((IServiceCollection)null!).AddCommandDispatcher());
        Assert.Throws<ArgumentNullException>(() => new ServiceCollection().AddCommandDispatcher((Assembly[])null!));
        Assert.Throws<ArgumentNullException>(() => ((IServiceCollection)null!).AddCommandDispatcher<SampleCommand>());
    }

    private sealed class ReplacementHandler : ICommandHandler<SampleCommand>
    {
        public Task HandleAsync(SampleCommand cmd, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class ReplacementDispatcher : ICommandDispatcher<SampleCommand>
    {
        public Task DispatchAsync(SampleCommand command, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}
