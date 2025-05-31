using DevCracks.Fractalize.Domain.Commands;
using DevCracks.Fractalize.Domain.Queries;
using DevCracks.Fractalize.Infrastructure.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Dev.Cracks.Fractalize.Infrastructure.Test;

public class QueryDispatcherTest
{
    [Fact]
    public async Task QueryDispatcher_WithValidQuery_Handles()
    {
        // Arrange
        var services = new ServiceCollection();
        services.TryAddTransient<IQueryHandler<SampleQuery, string>, SampleHandler>();
        services.AddQueryDispatchers();
        var queryDispatcher = new QueryDispatcher<SampleQuery, string>(services.BuildServiceProvider());

        // Act
        var result = await queryDispatcher.DispatchAsync(new SampleQuery());

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Sample Result", result);
    }

    [Fact]
    public async Task QueryDispatcher_WithValidConfiguration_Handles()
    {
        // Arrange
        var services = new ServiceCollection();
        services.TryAddQueryDispatcher<SampleQuery, string>();
        services.TryAddTransient<IQueryHandler<SampleQuery, string>, SampleHandler>();
        var serviceProvider = services.BuildServiceProvider();
        var queryDispatcher = serviceProvider.GetRequiredService<IQueryDispatcher<SampleQuery, string>>();

        // Act
        var result = await queryDispatcher.DispatchAsync(new SampleQuery());

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Sample Result", result);
    }

    [Fact]
    public async Task QueryHandler_WithValidConfiguration_Handles()
    {
        // Arrange
        var services = new ServiceCollection();
        services.TryAddQueryDispatcher<SampleQuery, string>();
        services.AddQueryHandlers();
        var serviceProvider = services.BuildServiceProvider();
        var queryDispatcher = serviceProvider.GetRequiredService<IQueryDispatcher<SampleQuery, string>>();

        // Act
        var result = await queryDispatcher.DispatchAsync(new SampleQuery());

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Sample Result", result);
    }
}

public class SampleQuery : IQuery<string>
{
}

public class SampleHandler : IQueryHandler<SampleQuery, string>
{
    public Task<string> HandleAsync(SampleQuery query, CancellationToken cancellationToken = default)
    {
        return Task.FromResult("Sample Result");
    }
}