using Efc.Fractalize.Application.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Efc.Fractalize.Application.Tests;

public class AddCqrsTest
{
    [Fact]
    public void AddCqrs_ReturnsTheSameServiceCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddCqrs();

        Assert.Same(services, result);
    }
}
