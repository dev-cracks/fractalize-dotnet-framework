# Contributing

Thanks for helping improve this project! Contributions of every size are welcome: typo fixes, clearer
documentation, bug reports, tests or new features.

## Ground rules

- **English only** for code, comments, documentation, commits, issues and pull requests.
- **Small, focused pull requests.** One topic per pull request; split large changes.
- **Tests come with the change.** New behaviour is covered by tests and bug fixes include a regression test.
- **Never commit secrets** or personal data. Use `.env.example` for configuration templates.

## Development setup

| Toolchain | Version |
| --- | --- |
| [.NET SDK](https://dotnet.microsoft.com/download) | 10.0 or later |
| .NET runtime | 10.0, to run the tests (the libraries target `net10.0`) |

The solution lives in [`src/Efc.Fractalize.Framework.sln`](src/Efc.Fractalize.Framework.sln):
the libraries are `Efc.Fractalize.Domain`, `Efc.Fractalize.Ddd`, `Efc.Fractalize.Application` and
`Efc.Fractalize.Infrastructure`, and their tests are under `src/Tests/`. The DDD core
(`Efc.Fractalize.Ddd`) must keep at least 90 % line coverage with its own tests, measured with
[`coverlet.ddd.runsettings`](coverlet.ddd.runsettings), which counts `[Obsolete]` code too; CI also rejects new
`[Obsolete]` or `[ExcludeFromCodeCoverage]` attributes in it.

```bash
# Restore, build with warnings as errors and run the tests before opening a pull request.
dotnet restore src/Efc.Fractalize.Framework.sln
dotnet build src/Efc.Fractalize.Framework.sln --no-restore -c Release -warnaserror
dotnet test src/Efc.Fractalize.Framework.sln --no-build -c Release --collect "XPlat Code Coverage" --settings coverlet.runsettings --results-directory TestResults
python3 tools/check_coverage.py TestResults --min 80
dotnet test src/Tests/Efc.Fractalize.Ddd.Tests --no-build -c Release --collect "XPlat Code Coverage" --settings coverlet.ddd.runsettings --results-directory TestResults/ddd
python3 tools/check_coverage.py TestResults/ddd --min 90

# With only a newer runtime installed (no 10.0 runtime), let the tests roll forward to it:
DOTNET_ROLL_FORWARD=Major dotnet test src/Efc.Fractalize.Framework.sln --no-build -c Release

# Markdown checks (Node.js required):
npx markdownlint-cli2 && python3 tools/check_links.py
```

Coding, testing, configuration and design rules: [`docs/engineering-standards.md`](docs/engineering-standards.md).

Repository-wide checks run in CI on every pull request:

| Check | What it verifies |
| --- | --- |
| `markdown` | Markdown style ([`.markdownlint.jsonc`](.markdownlint.jsonc)) and relative links ([`tools/check_links.py`](tools/check_links.py)) |
| `editorconfig` | Encoding, line endings, indentation and trailing whitespace ([`.editorconfig`](.editorconfig)) |
| `actionlint` | GitHub Actions workflow syntax and shell scripts |
| `powershell` | PowerShell syntax and PSScriptAnalyzer rules |
| `dotnet` | Restore, build with `-warnaserror` (code-style rules included), tests and minimum line coverage ([`tools/check_coverage.py`](tools/check_coverage.py)): 80 % for the solution and 90 % for `Efc.Fractalize.Ddd` |
| `conventional-commits` | Pull request title format |
| CodeQL (`actions`, `csharp`), dependency review | Security analysis and vulnerable dependencies |

## Commit messages and pull request titles

This repository follows [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/).
Releases, version numbers and `CHANGELOG.md` are generated from them, and CI rejects pull request titles
that don't comply.

```text
<type>(<optional scope>): <lowercase description>

feat(api): add pagination to the orders endpoint
fix(auth): refresh expired tokens before retrying
docs: explain how to run the project locally
```

| Type | Use it for | Release impact |
| --- | --- | --- |
| `feat` | a new feature | minor |
| `fix` | a bug fix | patch |
| `docs` | documentation only | none |
| `test`, `refactor`, `perf`, `style` | code changes without new behaviour | none |
| `build`, `ci`, `chore` | tooling, dependencies, pipelines | none |
| `revert` | reverting a previous commit | depends on the reverted commit |

Add `!` after the type (`feat!:`) and a `BREAKING CHANGE:` footer for breaking changes (major release).
Write a body that explains **what** changed and **why**, wrapped at 72 characters.

## Branches

Branch from `main` using the commit type as prefix: `feat/<topic>`, `fix/<topic>`, `docs/<topic>`,
`chore/<topic>`, `ci/<topic>`. Branch names describe the change, never the tool used to make it.

Use your own identity in every commit; if your global git identity belongs to another organization, set it
for this repository only: `git config user.name "<name>"` and `git config user.email "<email>"`.

## Pull request process

Nobody pushes to `main` directly, maintainers included: every change arrives through a pull request.

1. **Fork** the repository (or create a branch if you are a maintainer) from `main`.
2. Make your change with tests and run the checks above.
3. Open a pull request using the template. Its title must be a Conventional Commit. Pick a specific
   template when it fits: `feature.md`, `bugfix.md`, `documentation.md`, `maintenance.md` or `security.md`
   (`gh pr create --template feature.md`, or add `?template=feature.md` to the compare URL).
4. **Workflow approval.** For security, GitHub Actions do not run on pull requests from outside
   contributors until a maintainer approves them. Expect a short wait before CI starts.
5. **Merge requirements** on `main`:
   - Every required check is green, with no high-severity code scanning alerts.
   - A code owner has approved the latest changes and every review conversation is resolved.
   - **Commits are signed** ([how to sign commits](https://docs.github.com/en/authentication/managing-commit-signature-verification/signing-commits)).
     Maintainers can sign an existing branch without squashing it with
     [`scripts/fix-repo.ps1`](scripts/fix-repo.ps1).
   - Pull requests are merged with a merge commit or squashed (no rebase merges); the branch is deleted
     after merging.

## Releases

[release-please](https://github.com/googleapis/release-please) keeps a release pull request open with the
next [Semantic Versioning](https://semver.org/) number and the changelog. Merging it creates the `vX.Y.Z`
tag and the GitHub Release. Release tags are protected and can never be moved or deleted.

The NuGet packages are published by [`publish-nuget.yml`](.github/workflows/publish-nuget.yml) when library
code changes on `main` (version `1.0.<run number>`), and can also be published manually from the Actions tab.

## Code of Conduct

By participating you agree to follow the [Code of Conduct](CODE_OF_CONDUCT.md).
