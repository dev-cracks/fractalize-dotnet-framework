# Fractalize .NET Framework

[![CI](https://github.com/dev-cracks/fractalize-dotnet-framework/actions/workflows/ci.yml/badge.svg)](https://github.com/dev-cracks/fractalize-dotnet-framework/actions/workflows/ci.yml)
[![CodeQL](https://github.com/dev-cracks/fractalize-dotnet-framework/actions/workflows/codeql.yml/badge.svg)](https://github.com/dev-cracks/fractalize-dotnet-framework/actions/workflows/codeql.yml)
[![OpenSSF Scorecard](https://api.scorecard.dev/projects/github.com/dev-cracks/fractalize-dotnet-framework/badge)](https://scorecard.dev/viewer/?uri=github.com/dev-cracks/fractalize-dotnet-framework)
[![Latest release](https://img.shields.io/github/v/release/dev-cracks/fractalize-dotnet-framework?sort=semver)](https://github.com/dev-cracks/fractalize-dotnet-framework/releases)
[![NuGet](https://img.shields.io/nuget/vpre/Efc.Fractalize.Infrastructure.svg)](https://www.nuget.org/packages/Efc.Fractalize.Infrastructure)
[![NuGet downloads](https://img.shields.io/nuget/dt/Efc.Fractalize.Infrastructure.svg)](https://www.nuget.org/packages/Efc.Fractalize.Infrastructure)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Conventional Commits](https://img.shields.io/badge/Conventional%20Commits-1.0.0-fe5196.svg?logo=conventionalcommits&logoColor=white)](https://www.conventionalcommits.org/en/v1.0.0/)

> **An opinionated, open source .NET framework for building elegant, robust applications inspired by
> fractals.** Simple, repeated structures (entities, aggregates, domain events, commands and queries with
> their dispatchers) keep systems modular, scalable and easy to maintain.

## 📦 Packages

| Package | Contents |
| --- | --- |
| `Efc.Fractalize.Domain` | Building blocks with no dependencies: `EntityBase`, `AggregateRootBase`, `IDomainEvent`, `EventBase`, and the `ICommand` / `IQuery` contracts with their handlers and dispatchers |
| [`Efc.Fractalize.Ddd`](src/Efc.Fractalize.Ddd/README.md) | Domain-driven design core with no dependencies: `EntityBase` with identity equality, `AggregateRootBase` with domain events, `DomainEventBase`, `ValueObjectBase`, `DateRange`, and the `IReadRepository` / `IRepository` / `IUnitOfWorkBase` contracts. It overlaps with `Efc.Fractalize.Domain` until the maintainers decide which types survive |
| `Efc.Fractalize.Application` | `CommandDispatcher` and its fluent configurator, `CommandDispatcherConfigurator` (`AddCommandDispatcher`) |
| `Efc.Fractalize.Infrastructure` | `QueryDispatcher` and `QueryDispatcherConfigurator` (`AddQueryDispatcher`), `CqrsConfigurator` (`AddCqrsDispatchers`: commands and queries in one call) and `OptionsConfigurator` (`AddValidatedOptions`: options validated at startup). Depends on `Efc.Fractalize.Application` |

The libraries target `net10.0` and are published to [NuGet](https://www.nuget.org/packages/Efc.Fractalize.Infrastructure).

## 🚀 Getting started

```bash
dotnet add package Efc.Fractalize.Infrastructure
```

Define a query and its handler, register them and dispatch the query:

```csharp
using Efc.Fractalize.Domain.Queries;
using Efc.Fractalize.Infrastructure.Extensions;

public sealed record GetGreeting(string Name) : IQuery<string>;

public sealed class GetGreetingHandler : IQueryHandler<GetGreeting, string>
{
    public Task<string> HandleAsync(GetGreeting query, CancellationToken cancellationToken = default) =>
        Task.FromResult($"Hello, {query.Name}!");
}

// At startup: register the command and query dispatchers and the handlers of this assembly.
services.AddCqrsDispatchers(typeof(GetGreetingHandler).Assembly);

// Anywhere: resolve the dispatcher for the query and send it.
var greeting = await provider
    .GetRequiredService<IQueryDispatcher<GetGreeting, string>>()
    .DispatchAsync(new GetGreeting("Fractalize"));
```

Commands work the same way with `ICommand`, `ICommandHandler<TCommand>` and `ICommandDispatcher<TCommand>`. Each
configurator can also be used on its own: `AddCommandDispatcher(...)` (`Efc.Fractalize.Application.Commands`) and
`AddQueryDispatcher(...)` (`Efc.Fractalize.Infrastructure.Queries`) register the dispatcher and scan the handlers
of the given assemblies (every application dependency when none is given), and `AddCommandDispatcher<TCommand>()` /
`AddQueryDispatcher<TQuery, TResult>()` register the dispatcher of a single message. Every registration uses
`TryAdd*`, so a host can replace any implementation and calling a configurator twice is harmless.

Options are bound and validated with data annotations when the application starts, as the
[engineering standards](docs/engineering-standards.md#4-configuration-the-options-pattern) require:

```csharp
using Efc.Fractalize.Infrastructure.Extensions;

// From configuration (appsettings + environment variables)...
services.AddValidatedOptions<ShipmentRepositoryOptions>(configuration, ShipmentRepositoryOptions.SectionName);

// ...or from code (tests, other hosts, values read from a database).
services.AddValidatedOptions<ShipmentRepositoryOptions>(o => o.ConnectionString = connectionString);
```

### Migrating from 2.x helpers

The 2.x registration helpers still work with the same behaviour, but they are marked `[Obsolete]` and will be
removed in the next major version. Replace them as follows:

| 2.x helper (obsolete) | Replacement |
| --- | --- |
| `AddCqrs()` | `AddCqrsDispatchers(assemblies)` (`CqrsConfigurator`, `Efc.Fractalize.Infrastructure`) |
| `AddCommandHandlers()`, `AddCommandDispatchers()` | `AddCommandDispatcher(assemblies)` (`CommandDispatcherConfigurator`) |
| `TryAddCommandDispatcher<TCommand>()`, `ConfigureCommandHandler<TCommand>()` | `AddCommandDispatcher<TCommand>()` |
| `AddQueryHandlers()`, `AddQueryDispatchers()` | `AddQueryDispatcher(assemblies)` (`QueryDispatcherConfigurator`) |
| `TryAddQueryDispatcher<TQuery, TResult>()`, `ConfigureQueryHandler<TQuery, TResult>()` | `AddQueryDispatcher<TQuery, TResult>()` |
| `AddConfigurationSection<T>(configuration, sectionName)` | `AddValidatedOptions<T>(configuration, sectionName)` (`OptionsConfigurator`) |

Differences to keep in mind:

- The new configurators use `TryAdd*`: a handler or dispatcher registered earlier is kept, not duplicated.
- `AddCommandDispatcher(...)` and `AddQueryDispatcher(...)` also register the dispatcher for every message type, so
  the per-message `AddCommandDispatcher<TCommand>()` / `AddQueryDispatcher<TQuery, TResult>()` calls are optional.
- `AddValidatedOptions` always registers the options and validates them at startup: a missing or invalid section
  fails when the host starts, while `AddConfigurationSection` skipped missing sections and never validated.

## 🛠️ Building from source

Prerequisites, commands and the checks every pull request must pass are in
[`CONTRIBUTING.md`](CONTRIBUTING.md#development-setup). In short:

```bash
dotnet build src/Efc.Fractalize.Framework.sln -c Release -warnaserror
dotnet test src/Efc.Fractalize.Framework.sln -c Release
```

## 🛡️ Standards

- **[Engineering standards](docs/engineering-standards.md):** file-scoped namespaces and no unused code
  (build errors), minimum 80 % line coverage (CI gate), low coupling and high cohesion, Options pattern and
  fluent `*Configurator` classes.
- **English only** in code, comments, docs, commits and pull requests.
- **Conventional Commits** for every commit and pull request title; releases follow
  [Semantic Versioning](https://semver.org/) and [`CHANGELOG.md`](CHANGELOG.md) is generated by
  release-please.
- **Protected `main`:** pull requests only, required checks, code-owner approval, resolved conversations,
  signed commits, merge commit or squash (no rebase merges), no force pushes or deletion.
- **Protected release tags:** `v*` tags can't be moved or deleted.
- **Least-privilege workflows:** read-only token by default, explicit per-job permissions, every action
  pinned to a full commit SHA, checkouts without persisted credentials.
- **Supply chain:** CodeQL, dependency review, Dependabot and [OpenSSF Scorecard](https://scorecard.dev/).
- **Secrets never in Git:** secret scanning with push protection, `.env` files ignored.

These standards come from the [`public-repo-template`](https://github.com/danielfyo/public-repo-template).

## 🤝 Contributing

Read [`CONTRIBUTING.md`](CONTRIBUTING.md) before opening a pull request, follow the
[Code of Conduct](CODE_OF_CONDUCT.md), and report vulnerabilities privately as described in
[`SECURITY.md`](SECURITY.md). For questions, see [`SUPPORT.md`](SUPPORT.md).

## 📄 License

[MIT](LICENSE) © 2025 Dev Cracks
