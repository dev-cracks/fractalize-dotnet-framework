# Instructions for AI coding agents

Follow [`docs/engineering-standards.md`](docs/engineering-standards.md) and
[`CONTRIBUTING.md`](CONTRIBUTING.md) for every change. In short:

- File-scoped namespaces; no unused variables, parameters, members or usings (the build fails otherwise).
- Tests with every change; keep line coverage at or above the CI floor (target 80 %), core and
  sensitive code first.
- One responsibility per component, consumed through its interface.
- Configuration through options classes bound and validated in the component's `*Configurator`, which
  calls the configurator of the layer below (`Repository` → `Service` → API).
- English in code, comments, commits and pull requests; Conventional Commits; never commit secrets.
