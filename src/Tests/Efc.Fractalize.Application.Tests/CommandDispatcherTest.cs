using Efc.Fractalize.Application.Commands;
using Efc.Fractalize.Domain.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Efc.Fractalize.Application.Tests;

public class CommandDispatcherTest
{
    [Fact]
    public async Task CommandDispatcher_WithValidCommand_Handles()
    {
        // Arrange
        var handler = new SampleHandler();
        var services = new ServiceCollection();
        services.TryAddSingleton<ICommandHandler<SampleCommand>>(handler);
        var commandDispatcher = new CommandDispatcher<SampleCommand>(services.BuildServiceProvider());

        // Act
        await commandDispatcher.DispatchAsync(new SampleCommand());

        // Assert
        Assert.Equal(1, handler.Calls);
    }

    [Fact]
    public async Task CommandDispatcher_WithoutHandler_Throws()
    {
        var commandDispatcher = new CommandDispatcher<SampleCommand>(new ServiceCollection().BuildServiceProvider());

        await Assert.ThrowsAsync<InvalidOperationException>(() => commandDispatcher.DispatchAsync(new SampleCommand()));
    }

    [Fact]
    public async Task CommandDispatcher_WithValidConfiguration_Handles()
    {
        // Arrange
        var handler = new SampleHandler();
        var services = new ServiceCollection();
        services.AddCommandDispatcher<SampleCommand>();
        services.TryAddSingleton<ICommandHandler<SampleCommand>>(handler);
        var commandDispatcher = services.BuildServiceProvider()
            .GetRequiredService<ICommandDispatcher<SampleCommand>>();

        // Act
        await commandDispatcher.DispatchAsync(new SampleCommand());

        // Assert
        Assert.Equal(1, handler.Calls);
    }
}

public class SampleCommand : ICommand
{
}

public class SampleHandler : ICommandHandler<SampleCommand>
{
    public int Calls { get; private set; }

    public Task HandleAsync(SampleCommand cmd, CancellationToken cancellationToken = default)
    {
        Calls++;
        return Task.CompletedTask;
    }
}
