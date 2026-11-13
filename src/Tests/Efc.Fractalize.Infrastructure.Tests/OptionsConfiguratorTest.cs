using System.ComponentModel.DataAnnotations;
using Efc.Fractalize.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Efc.Fractalize.Infrastructure.Tests;

public class OptionsConfiguratorTest
{
    private const string SectionName = "Shipments:Repository";

    [Fact]
    public void AddValidatedOptions_FromValidConfiguration_BindsAndPassesStartupValidation()
    {
        var services = new ServiceCollection();

        var result = services.AddValidatedOptions<RepositoryOptions>(
            Configuration(("ConnectionString", "Server=db"), ("CommandTimeoutSeconds", "60")), SectionName);
        using var provider = Build(services);

        Assert.Same(services, result);
        provider.GetRequiredService<IStartupValidator>().Validate();
        var options = provider.GetRequiredService<IOptions<RepositoryOptions>>().Value;
        Assert.Equal("Server=db", options.ConnectionString);
        Assert.Equal(60, options.CommandTimeoutSeconds);
    }

    [Fact]
    public void AddValidatedOptions_FromInvalidConfiguration_FailsAtStartup()
    {
        var services = new ServiceCollection();

        services.AddValidatedOptions<RepositoryOptions>(Configuration(("CommandTimeoutSeconds", "0")), SectionName);
        using var provider = Build(services);

        var exception = Assert.Throws<OptionsValidationException>(
            () => provider.GetRequiredService<IStartupValidator>().Validate());
        Assert.Contains(exception.Failures, f => f.Contains(nameof(RepositoryOptions.ConnectionString)));
        Assert.Contains(exception.Failures, f => f.Contains(nameof(RepositoryOptions.CommandTimeoutSeconds)));
    }

    [Fact]
    public void AddValidatedOptions_FromMissingSection_FailsAtStartup()
    {
        var services = new ServiceCollection();

        services.AddValidatedOptions<RepositoryOptions>(new ConfigurationBuilder().Build(), SectionName);
        using var provider = Build(services);

        Assert.Throws<OptionsValidationException>(() => provider.GetRequiredService<IStartupValidator>().Validate());
    }

    [Fact]
    public void AddValidatedOptions_FromValidCode_ConfiguresAndPassesStartupValidation()
    {
        var services = new ServiceCollection();

        var result = services.AddValidatedOptions<RepositoryOptions>(o => o.ConnectionString = "Server=code");
        using var provider = Build(services);

        Assert.Same(services, result);
        provider.GetRequiredService<IStartupValidator>().Validate();
        var options = provider.GetRequiredService<IOptions<RepositoryOptions>>().Value;
        Assert.Equal("Server=code", options.ConnectionString);
        Assert.Equal(30, options.CommandTimeoutSeconds);
    }

    [Fact]
    public void AddValidatedOptions_FromInvalidCode_FailsAtStartupAndOnFirstUse()
    {
        var services = new ServiceCollection();

        services.AddValidatedOptions<RepositoryOptions>(o => o.CommandTimeoutSeconds = 301);
        using var provider = Build(services);

        Assert.Throws<OptionsValidationException>(() => provider.GetRequiredService<IStartupValidator>().Validate());
        Assert.Throws<OptionsValidationException>(() => provider.GetRequiredService<IOptions<RepositoryOptions>>().Value);
    }

    [Fact]
    public void AddValidatedOptions_WithInvalidArguments_Throws()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        Assert.Throws<ArgumentNullException>(
            () => services.AddValidatedOptions<RepositoryOptions>((IConfiguration)null!, SectionName));
        Assert.Throws<ArgumentException>(() => services.AddValidatedOptions<RepositoryOptions>(configuration, " "));
        Assert.Throws<ArgumentNullException>(
            () => services.AddValidatedOptions<RepositoryOptions>((Action<RepositoryOptions>)null!));
        Assert.Throws<ArgumentNullException>(
            () => ((IServiceCollection)null!).AddValidatedOptions<RepositoryOptions>(_ => { }));
    }

    private static ServiceProvider Build(IServiceCollection services) =>
        services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });

    private static IConfiguration Configuration(params (string Key, string Value)[] values) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(values.Select(v => KeyValuePair.Create<string, string?>($"{SectionName}:{v.Key}", v.Value)))
            .Build();

    public sealed class RepositoryOptions
    {
        [Required]
        public string ConnectionString { get; set; } = string.Empty;

        [Range(1, 300)]
        public int CommandTimeoutSeconds { get; set; } = 30;
    }
}
