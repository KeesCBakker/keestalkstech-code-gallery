# Migrating xUnit and FluentAssertions to TUnit

This runbook supplements the official documentation with practical steps for migrating a .NET test project:

- From xUnit to TUnit and Microsoft Testing Platform (MTP)
- From FluentAssertions to TUnit's built-in assertions

Do not duplicate TUnit's current API guidance here. Use the official documentation as the source of truth:

- [Install TUnit](https://tunit.dev/docs/getting-started/installation/)
- [Migrate from xUnit](https://tunit.dev/docs/migration/xunit/)

Package versions and migration syntax change over time. Follow those pages instead of copying versions or conversion tables from an older migration.

## Before migration

Create a feature branch and confirm that the worktree is clean:

```console
git status --short
git switch -c feature/migrate-tests-to-tunit
```

Do not continue until `git status --short` returns no output. Choose another branch name if this one already exists.

Inventory the current test setup:

```console
rg -n -i "xunit|\[(Fact|Theory|InlineData|MemberData|ClassData|Trait|Collection|CollectionDefinition)|I(Class|Collection)Fixture|IAsyncLifetime|ITestOutputHelper|FluentAssertions|coverlet|Microsoft\.NET\.Test\.Sdk" .
```

Record before making changes:

- Build warnings and errors
- Discovered, passed, failed, and skipped test counts
- Unit- and integration-test durations
- Existing coverage output
- Fixtures, inherited tests, traits, data sources, and custom assertions

Capture performance using the method described in [Compare performance](#compare-performance). Benchmark container-backed tests separately.

## Migrate the test framework

Follow the official [installation](https://tunit.dev/docs/getting-started/installation/) and [xUnit migration](https://tunit.dev/docs/migration/xunit/) guides.

Important migration conventions:

- Keep xUnit installed while running TUnit's migration analyzer.
- Review every generated change; the analyzer cannot infer intended assertion semantics or fixture lifetime.
- Configure migrated test projects as executables.
- Use the same TUnit version as other migrated projects unless performing a coordinated upgrade.
- Add every migrated test project to the repository's solution or solution file.
- Remove xUnit, its runner, `Microsoft.NET.Test.Sdk`, `coverlet.collector`, `coverlet.msbuild`, and xUnit global usings after conversion.
- Keep one migration commit per test project where practical.

To make solution-wide `dotnet test` use MTP, add this repository-level `global.json` configuration:

```json
{
  "test": {
    "runner": "Microsoft.Testing.Platform"
  }
}
```

This setting selects the test runner but does not pin a .NET SDK. Keep SDK selection separate unless the repository intentionally requires an exact SDK policy. An SDK pin can affect local development, CI, and Docker builds; Dockerfiles that run TUnit projects directly with `dotnet run` do not need this runner setting.

Solution-wide MTP execution requires the .NET 10 SDK or newer and every included test project must support MTP. Verify the active SDK with `dotnet --version`.

Do not suppress TUnit analyzers to make converted code compile. Fix the underlying data source, lifecycle, output capture, or assertion instead.

## Remove FluentAssertions

FluentAssertions is a separate migration step. TUnit's xUnit analyzer does not guarantee complete conversion of FluentAssertions chains or custom extensions.

Inventory all usages:

```console
rg -n -i "FluentAssertions|\.Should\(\)|AndConstraint|ReferenceTypeAssertions" .
```

Convert each assertion to the current TUnit assertion API. Preserve semantics rather than translating method names mechanically:

- Use equality for scalar and value comparisons.
- Use equivalence only when structural equivalence is intended.
- Preserve all meaningful exception checks, such as type, parameter name, and inner exception.
- Avoid exact locale-dependent exception messages unless the message is the contract.
- Custom assertion helpers that execute TUnit assertions must return `Task`, and callers must await them. Pure formatting or predicate helpers can remain synchronous.

Remove the FluentAssertions package only after the repository-wide search returns no usages.

## Review lifecycle and parallelism

Pay special attention to behavior that differs between runners:

- Convert xUnit fixtures using the current TUnit lifecycle guidance.
- Verify fixture sharing and disposal explicitly.
- For expensive asynchronous fixtures, make partially initialized fields nullable and perform cleanup in `try`/`finally` so initialization failures are safe.
- Add `[InheritsTests]` where a concrete class must discover tests declared by a base class, then verify those tests are listed.
- Audit static configuration, culture, environment variables, files, ports, factories, and containers.
- Use targeted parallelism constraints only where shared state requires them.

TUnit captures console output per test. Do not redirect process-wide output with `Console.SetOut` or suppress `TUnit0055`. The following readback pattern works with the TUnit version used when this guide was written; verify the current output API when upgrading TUnit:

```csharp
await app.Execute(["Kees"]);

var output = TestContext.Current!.GetStandardOutput().TrimEnd();
await Assert.That(output).IsEqualTo("Hi Kees!");
```

If a logger is needed only as a constructor dependency, prefer `NullLogger<T>.Instance` over retaining a mocking package for incidental logging assertions.

## Update filters and automation

Replace xUnit traits with TUnit properties and replace VSTest filters with MTP tree-node filters. Consult the current [TUnit filter documentation](https://tunit.dev/docs/execution/test-filters) for exact syntax.

Migration requirements:

- Keep unit and integration filters separate.
- Define what `Integration` means for the repository and tag every test that belongs to that boundary. Do not assume a class name such as `IntegrationTests` is automatically filtered.
- Put runner arguments after `--` when using `dotnet run`.
- Set `--minimum-expected-tests` so a broken filter cannot silently run zero tests.
- Update Dockerfiles, generated Dockerfile templates, CI workflows, scripts, and documentation.
- Use `--no-build` only when the same project and configuration were built earlier.

Command templates, using `Category=Integration` as an example convention:

```console
dotnet run --project path/to/Project.Tests.csproj --configuration Release -- --treenode-filter "/**[Category!=Integration]" --minimum-expected-tests 10
```

```console
dotnet run --project path/to/Project.Tests.csproj --configuration Release -- --treenode-filter "/**[Category=Integration]" --minimum-expected-tests 5
```

Replace the example counts with the recorded counts for that project.

## Migrate coverage

Remove `coverlet.collector` and `coverlet.msbuild`, then use the coverage support provided through the current TUnit/MTP setup. Also inspect Dockerfiles and build scripts for obsolete `coverlet.console` installations.

Migration requirements:

- Produce one uniquely named Cobertura report per test project.
- Remove stale reports before each run.
- Keep optional integration coverage separate.
- Verify each report is non-empty and contains the intended production assembly.
- Fail the build when the test command fails or the expected report is missing.
- Integrate the command into the repository's existing build or CI automation. If none exists, document a repeatable command before adding a new script.

After deleting that project's stale report, run:

```console
dotnet run --project path/to/Project.Tests.csproj --configuration Release -- --coverage --coverage-output TestResults/coverage/project.cobertura.xml --coverage-output-format cobertura --minimum-expected-tests 10 --disable-logo
```

Repeat this for every migrated project with a unique output filename and the project's recorded test count. Make the repository's build or CI automation fail when the command fails, the report is missing or empty, or the Cobertura XML does not contain the intended production assembly. A successful process exit alone does not prove useful coverage.

## Verify the result

Run build and tests sequentially. Running them concurrently against the same output directories can produce misleading file-lock warnings.

```console
dotnet --version
dotnet restore path/to/Repository.sln
dotnet build path/to/Repository.sln --configuration Release --no-incremental
dotnet test --solution path/to/Repository.sln --configuration Release --no-build -- --disable-logo
```

For every project, list tests and run with its known expected count:

```console
dotnet run --project path/to/Project.Tests.csproj --configuration Release --no-build -- --list-tests
dotnet run --project path/to/Project.Tests.csproj --configuration Release --no-build -- --minimum-expected-tests 10 --disable-logo
```

Replace `10` with that project's recorded test count.

Confirm:

- The build has zero warnings and errors.
- Test counts reconcile exactly with the baseline. Explain every intentional difference; a higher count can indicate duplicate discovery.
- Inherited tests run for every intended concrete class.
- Unit filters do not start containers.
- Integration filters select only integration tests.
- Repeated runs do not expose shared-state races.
- Coverage reports are current and valid.
- Dependency vulnerability checks are clean.
- No obsolete framework or assertion references remain.

```console
rg -n -i "xunit|\[(Fact|Theory|InlineData|MemberData|ClassData|Trait|Collection|CollectionDefinition)|I(Class|Collection)Fixture|IAsyncLifetime|ITestOutputHelper|FluentAssertions|coverlet\.(collector|msbuild|console)|Microsoft\.NET\.Test\.Sdk|TUnit0055" .
dotnet list path/to/Project.Tests.csproj package --vulnerable --include-transitive
```

Run the package audit for every migrated project, not just one representative project.

## Compare performance

Repeat the baseline method on the migrated branch:

1. Use the same machine and Release configuration.
2. Build outside the timed section.
3. Run one warm-up.
4. Measure at least five equivalent executions.
5. Report median, range, environment, and test count.
6. Report integration tests separately.

If no baseline was captured, use a temporary detached Git worktree at the exact base commit. Do not compare unrelated revisions or unequal commands.

## Publish

Before pushing:

```console
git diff --check
git status --short
git fetch origin
git diff origin/main...HEAD --check
```

Require `git status --short` to return no output before publishing.

The pull request should include:

- Baseline and final test counts
- Build and coverage verification
- Performance methodology and results
- Integration-test status
- Remaining risks or follow-up work
