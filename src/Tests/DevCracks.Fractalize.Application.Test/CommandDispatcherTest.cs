using DevCracks.Fractalize.Application.Commands;
using DevCracks.Fractalize.Domain.Commands;
using DevCracks.Fractalize.Domain.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Dev.Cracks.Fractalize.Application.Test;

public class QueryDispatcherTest
{
    [Fact]
    public async Task CommandDispatcher_WithValidCommand_Handles()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddCommandDispatchers();
        services.TryAddTransient<ICommandHandler<SampleCommand>, SampleHandler>();
        var commandDispatcher = new CommandDispatcher<SampleCommand>(services.BuildServiceProvider());

        // Act
        await commandDispatcher.DispatchAsync(new SampleCommand());

        // Assert
        Assert.NotNull(commandDispatcher);
    }

    [Fact]
    public async Task CommandDispatcher_WithValidConfiguration_Handles()
    {
        // Arrange
        var services = new ServiceCollection();
        services.TryAddCommandDispatcher<SampleCommand>();
        services.TryAddTransient<ICommandHandler<SampleCommand>, SampleHandler>();
        var serviceProvider = services.BuildServiceProvider();
        var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher<SampleCommand>>();

        // Act
        await commandDispatcher.DispatchAsync(new SampleCommand());

        // Assert
        Assert.NotNull(commandDispatcher);
    }

    [Fact]
    public async Task CommandHandler_WithValidConfiguration_Handles()
    {
        // Arrange
        var services = new ServiceCollection();
        services.TryAddCommandDispatcher<SampleCommand>();
        services.AddCommandHandlers();
        var serviceProvider = services.BuildServiceProvider();
        var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher<SampleCommand>>();

        // Act
        await commandDispatcher.DispatchAsync(new SampleCommand());

        // Assert
        Assert.NotNull(commandDispatcher);
    }
}

public class SampleCommand : ICommand
{
}

public class SampleHandler : ICommandHandler<SampleCommand>
{
    public Task HandleAsync(SampleCommand cmd, CancellationToken cancellationToken = default)
    {
        return Task.FromResult("Sample Result");
    }
}