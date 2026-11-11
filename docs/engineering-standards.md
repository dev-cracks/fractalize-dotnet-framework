# Engineering standards

These rules apply to every change in this repository. Most of them are enforced by the build
([`.editorconfig`](../.editorconfig), [`Directory.Build.props`](../Directory.Build.props)) and by CI; the
rest are checked in code review.

## 1. Code style

| Rule | Enforced by |
| --- | --- |
| **File-scoped namespaces** (`namespace Company.Product.Feature;`) in every C# file | `IDE0161` as error |
| **No unused code**: variables, parameters, private members, assignments and `using` directives | `IDE0059`, `IDE0060`, `IDE0051`, `IDE0052`, `IDE0005`, `CS0168`, `CS0219` as errors |
| One public type per file, file named after the type | Review |
| Nullable reference types enabled | `Directory.Build.props` |

`EnforceCodeStyleInBuild` is on, so these rules break `dotnet build` locally and in CI, not only in the IDE.
Run `dotnet format` before committing to fix most of them automatically.

## 2. Tests and coverage

- **Minimum 80 % line coverage** for the whole solution, measured in CI with coverlet
  ([`coverlet.runsettings`](../coverlet.runsettings)) and checked by
  [`tools/check_coverage.py`](../tools/check_coverage.py).
- **Priority:** the domain and application core first, then sensitive code (authentication,
  authorization, cryptography, money, personal data, tenancy isolation, validation of external input).
  Those areas aim for 100 % and must cover the failure paths, not only the happy path.
- Every bug fix ships with a regression test; every new behaviour ships with its tests.
- Unit tests are fast, deterministic and isolated: no network, real clock or shared state. Depend on
  abstractions (`TimeProvider`, `IOptions<T>`, repository interfaces) so they can be replaced in tests.
- Excluded from coverage: generated code, EF Core migrations and `[ExcludeFromCodeCoverage]` (use it only
  for composition roots such as `Program.cs`, never to hide untested logic).

Run it locally:

```bash
dotnet test --collect "XPlat Code Coverage" --settings coverlet.runsettings --results-directory TestResults
python3 tools/check_coverage.py TestResults --min 80
```

When a repository is still below 80 %, CI enforces its current floor (`COVERAGE_MIN` in
[`ci.yml`](../.github/workflows/ci.yml)). The floor only goes up: raise it in every pull request that
improves coverage, until it reaches 80.

## 3. Design: low coupling, high cohesion

- A component does **one thing** and exposes it through an interface (`IShipmentRepository`,
  `IShipmentService`). Consumers depend on the interface, never on the implementation.
- Dependencies point inwards: API → application → domain; infrastructure implements interfaces declared
  by the application or the domain.
- No static mutable state and no service locator (`IServiceProvider.GetService` inside business code).
- Constructor injection only; a constructor with more than ~5 dependencies is a sign the class does too
  much.
- Every component can be moved to another project or solution together with its configurator (below)
  without touching anything else.

## 4. Configuration: the Options pattern

Every configurable value lives in a strongly typed options class, never read with
`IConfiguration["..."]` inside business code.

```csharp
namespace Fractalize.Shipments.Infrastructure;

public sealed class ShipmentRepositoryOptions
{
    public const string SectionName = "Shipments:Repository";

    [Required]
    public string ConnectionString { get; set; } = string.Empty;

    [Range(1, 300)]
    public int CommandTimeoutSeconds { get; set; } = 30;
}
```

- **Sources.** `appsettings.json` holds defaults and non-secret values. Secrets and per-environment
  values come from **environment variables** (`Shipments__Repository__ConnectionString`) or user
  secrets, which ASP.NET Core binds to the same section. Values stored in a **database** are loaded by
  an `IConfigureOptions<T>` (or a custom configuration provider) registered by the same configurator,
  so consumers still receive `IOptions<T>` / `IOptionsMonitor<T>` and don't know where the value came
  from.
- **Validation.** Data annotations or `.Validate(...)`, always with `.ValidateOnStart()`: a
  misconfigured service fails at startup, not on the first request.
- **Secrets** are never committed: their keys exist in `appsettings.json` with empty values.

## 5. Composition: fluent configurators

Each layer of a component has an interface, an implementation and a **configurator**: a static class
with `Add*` extension methods that registers the component, binds and validates **its own** options, and
calls the configurator of the layer it depends on. Configurators chain up to the API, so each piece can
be reused or moved on its own.

```text
Program.cs ──▶ ShipmentApiConfigurator.AddShipmentApi()
                 └─▶ ShipmentServiceConfigurator.AddShipmentService()      (ShipmentServiceOptions)
                       └─▶ ShipmentRepositoryConfigurator.AddShipmentRepository() (ShipmentRepositoryOptions)
```

```csharp
namespace Fractalize.Shipments.Infrastructure;

public static class ShipmentRepositoryConfigurator
{
    // From configuration (appsettings + environment variables).
    public static IServiceCollection AddShipmentRepository(
        this IServiceCollection services, IConfiguration configuration) =>
        services.AddShipmentRepository(configuration.GetSection(ShipmentRepositoryOptions.SectionName).Bind);

    // From code (tests, other hosts, values read from a database).
    public static IServiceCollection AddShipmentRepository(
        this IServiceCollection services, Action<ShipmentRepositoryOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        services.AddOptions<ShipmentRepositoryOptions>()
            .Configure(configure)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.TryAddScoped<IShipmentRepository, ShipmentRepository>();
        return services;
    }
}
```

```csharp
namespace Fractalize.Shipments.Application;

public static class ShipmentServiceConfigurator
{
    public static IServiceCollection AddShipmentService(
        this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<ShipmentServiceOptions>()
            .Bind(configuration.GetSection(ShipmentServiceOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.TryAddScoped<IShipmentService, ShipmentService>();

        // The service brings its own dependencies: callers only need this one line.
        return services.AddShipmentRepository(configuration);
    }
}
```

```csharp
// Program.cs: the API only composes top-level modules.
builder.Services
    .AddShipmentApi(builder.Configuration)
    .AddCustomerApi(builder.Configuration);
```

Rules for configurators:

- Named `<Component>Configurator`, placed next to the component, method `Add<Component>`.
- Return `IServiceCollection` so calls chain fluently.
- Use `TryAdd*` so a host can replace any implementation before or after calling the configurator,
  and calling a configurator twice is harmless.
- Offer an `IConfiguration` overload and an `Action<TOptions>` overload.
- Only the configurator knows the implementation type: keep implementations `internal` where the
  project layout allows it.
- Every configurator has a test that builds the container (`ValidateOnBuild = true`) and resolves the
  component with valid and with invalid options.

## 6. Pull request checklist

- [ ] Build has no errors or warnings (`dotnet build -warnaserror`).
- [ ] New and changed code is covered by tests; coverage did not decrease.
- [ ] New configuration uses an options class, bound and validated by the component's configurator.
- [ ] No secrets, connection strings or personal data in code, configuration or tests.
