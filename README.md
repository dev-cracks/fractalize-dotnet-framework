# fractalize-dotnet-framework

[![CI](https://github.com/dev-cracks/fractalize-dotnet-framework/actions/workflows/ci.yml/badge.svg)](https://github.com/dev-cracks/fractalize-dotnet-framework/actions/workflows/ci.yml)
[![CodeQL](https://github.com/dev-cracks/fractalize-dotnet-framework/actions/workflows/codeql.yml/badge.svg)](https://github.com/dev-cracks/fractalize-dotnet-framework/actions/workflows/codeql.yml)
[![OpenSSF Scorecard](https://api.scorecard.dev/projects/github.com/dev-cracks/fractalize-dotnet-framework/badge)](https://scorecard.dev/viewer/?uri=github.com/dev-cracks/fractalize-dotnet-framework)
[![Publish NuGet Package](https://github.com/dev-cracks/fractalize-dotnet-framework/actions/workflows/publish-nuget.yml/badge.svg)](https://github.com/dev-cracks/fractalize-dotnet-framework/actions/workflows/publish-nuget.yml)
[![NuGet](https://img.shields.io/nuget/vpre/DevCracks.Fractalize.Infrastructure.svg)](https://www.nuget.org/packages/DevCracks.Fractalize.Infrastructure)
[![NuGet downloads](https://img.shields.io/nuget/dt/DevCracks.Fractalize.Infrastructure.svg)](https://www.nuget.org/packages/DevCracks.Fractalize.Infrastructure)
[![MyGet](https://img.shields.io/myget/dev-cracks/v/DevCracks.Fractalize.Infrastructure.svg)](https://myget.org/gallery/dev-cracks)
[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4.svg?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Conventional Commits](https://img.shields.io/badge/Conventional%20Commits-1.0.0-fe5196.svg?logo=conventionalcommits&logoColor=white)](https://www.conventionalcommits.org/en/v1.0.0/)

> **A modern .NET framework inspired by fractals**: simple, repeated structures (entities, aggregates,
> domain events, commands and queries) that compose into scalable, maintainable systems through modular
> components and solid patterns.

## 📦 Packages

| Project | Contents |
| --- | --- |
| `DevCracks.Fractalize.Domain` | `EntityBase<TId>`, `AggregateRootBase<TId>`, `EventBase`, command and query contracts |
| `DevCracks.Fractalize.Application` | Command dispatcher and handler registration (`AddCqrs`) |
| `DevCracks.Fractalize.Infrastructure` | Query dispatcher and `AddConfigurationSection<T>` (published to NuGet) |

```csharp
builder.Services
    .AddCqrs()
    .AddConfigurationSection<MyOptions>(builder.Configuration, "MySection");
```

## 🛡️ Standards

- [Engineering standards](docs/engineering-standards.md): file-scoped namespaces, no unused code,
  minimum 80 % line coverage, Options pattern and fluent configurators.
- Conventional Commits, protected `main`, signed commits and automated releases, as described in
  [`CONTRIBUTING.md`](CONTRIBUTING.md).

## 🤝 Contributing

Read [`CONTRIBUTING.md`](CONTRIBUTING.md) before opening a pull request, and report vulnerabilities
privately as described in [`SECURITY.md`](SECURITY.md).

## 📄 License

[MIT](LICENSE) © Dev Cracks
