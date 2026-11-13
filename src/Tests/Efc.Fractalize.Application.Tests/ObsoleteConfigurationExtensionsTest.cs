using Efc.Fractalize.Application.Commands;
using Efc.Fractalize.Application.Extensions;
using Efc.Fractalize.Domain.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Efc.Fractalize.Application.Tests;

// These tests cover the 2.x registration helpers on purpose: they must keep working until they are removed.
#pragma warning disable CS0618 // Type or member is obsolete
public class ObsoleteConfigurationExtensionsTest
{
    [Fact]
    public void AddCqrs_RegistersCommandHandlers()
    {
        var services = new ServiceCollection();

        var result = services.AddCqrs();

        Assert.Same(services, result);
        Assert.IsType<SampleHandler>(
            services.BuildServiceProvider().GetRequiredService<ICommandHandler<SampleCommand>>());
    }

    [Fact]
    public void AddCommandDispatchers_ReturnsTheSameServiceCollection()
    {
        var services = new ServiceCollection();

        Assert.Same(services, services.AddCommandDispatchers());
    }

    [Fact]
    public async Task TryAddCommandDispatcher_WithHandlers_Dispatches()
    {
        var services = new ServiceCollection();
        services.TryAddCommandDispatcher<SampleCommand>();
        services.AddCommandHandlers();
        var dispatcher = services.BuildServiceProvider().GetRequiredService<ICommandDispatcher<SampleCommand>>();

        await dispatcher.DispatchAsync(new SampleCommand());

        Assert.IsType<CommandDispatcher<SampleCommand>>(dispatcher);
    }

    [Fact]
    public void ConfigureCommandHandler_RegistersTheCommandDispatcher()
    {
        var services = new ServiceCollection();

        var result = services.ConfigureCommandHandler<SampleCommand>();

        Assert.Same(services, result);
        Assert.Contains(services, d => d.ServiceType == typeof(ICommand));
        Assert.Contains(services, d => d.ServiceType == typeof(ICommandDispatcher<SampleCommand>));
    }
}
#pragma warning restore CS0618
