using Efc.Fractalize.Application.Commands;
using Efc.Fractalize.Domain.Commands;
using Efc.Fractalize.Domain.Queries;
using Efc.Fractalize.Infrastructure.Extensions;
using Efc.Fractalize.Infrastructure.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Efc.Fractalize.Infrastructure.Tests;

public class CqrsConfiguratorTest
{
    [Fact]
    public async Task AddCqrsDispatchers_BuildsAndResolvesCommandAndQueryDispatchers()
    {
        var services = new ServiceCollection();

        var result = services.AddCqrsDispatchers(typeof(CqrsConfiguratorTest).Assembly);
        using var provider = services.BuildServiceProvider(
            new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });

        Assert.Same(services, result);
        var commandDispatcher = provider.GetRequiredService<ICommandDispatcher<RenameCommand>>();
        Assert.IsType<CommandDispatcher<RenameCommand>>(commandDispatcher);
        await commandDispatcher.DispatchAsync(new RenameCommand());
        var queryDispatcher = provider.GetRequiredService<IQueryDispatcher<SampleQuery, string>>();
        Assert.IsType<QueryDispatcher<SampleQuery, string>>(queryDispatcher);
        Assert.Equal("Sample Result", await queryDispatcher.DispatchAsync(new SampleQuery()));
    }

    [Fact]
    public void AddCqrsDispatchers_WithNullServices_Throws() =>
        Assert.Throws<ArgumentNullException>(() => ((IServiceCollection)null!).AddCqrsDispatchers());
}

public class RenameCommand : ICommand
{
}

public class RenameCommandHandler : ICommandHandler<RenameCommand>
{
    public Task HandleAsync(RenameCommand command, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
