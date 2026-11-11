using DevCracks.Fractalize.Application.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Dev.Cracks.Fractalize.Application.Test;

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
