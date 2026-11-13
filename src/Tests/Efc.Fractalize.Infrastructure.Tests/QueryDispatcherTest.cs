using Efc.Fractalize.Domain.Queries;
using Efc.Fractalize.Infrastructure.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Efc.Fractalize.Infrastructure.Tests;

public class QueryDispatcherTest
{
    [Fact]
    public async Task QueryDispatcher_WithValidQuery_Handles()
    {
        // Arrange
        var services = new ServiceCollection();
        services.TryAddTransient<IQueryHandler<SampleQuery, string>, SampleHandler>();
        var queryDispatcher = new QueryDispatcher<SampleQuery, string>(services.BuildServiceProvider());

        // Act
        var result = await queryDispatcher.DispatchAsync(new SampleQuery());

        // Assert
        Assert.Equal("Sample Result", result);
    }

    [Fact]
    public async Task QueryDispatcher_WithoutHandler_Throws()
    {
        var queryDispatcher = new QueryDispatcher<SampleQuery, string>(new ServiceCollection().BuildServiceProvider());

        await Assert.ThrowsAsync<InvalidOperationException>(() => queryDispatcher.DispatchAsync(new SampleQuery()));
    }

    [Fact]
    public async Task QueryDispatcher_WithValidConfiguration_Handles()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddQueryDispatcher<SampleQuery, string>();
        services.TryAddTransient<IQueryHandler<SampleQuery, string>, SampleHandler>();
        var queryDispatcher = services.BuildServiceProvider().GetRequiredService<IQueryDispatcher<SampleQuery, string>>();

        // Act
        var result = await queryDispatcher.DispatchAsync(new SampleQuery());

        // Assert
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
