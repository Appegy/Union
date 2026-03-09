# External Integrations

**Analysis Date:** 2026-03-09

## APIs & External Services

**None.** This is a self-contained C# source generator library with no external API calls, HTTP clients, or third-party service integrations.

## Data Storage

**Databases:**
- None

**File Storage:**
- Local filesystem only
- The build process copies generated DLL/PDB to `Runtime/` via MSBuild `CustomAfterBuild` target in `Appegy.Union.Generator~/Appegy.Union.Generator/Appegy.Union.Generator.csproj`

**Caching:**
- None (Roslyn's incremental generator pipeline handles its own caching internally)

## Authentication & Identity

**Auth Provider:**
- Not applicable - this is a compile-time code generation library

## Monitoring & Observability

**Error Tracking:**
- None

**Logs:**
- No runtime logging
- The generator reports issues via Roslyn Diagnostics (`DiagnosticDescriptors.cs` at `Appegy.Union.Generator~/Appegy.Union.Generator/DiagnosticDescriptors.cs`)

## CI/CD & Deployment

**Hosting:**
- Distributed as a Unity Package (UPM) via GitHub repository
- Package manifest: `package.json`
- Documentation URL: https://github.com/Appegy/Union
- Changelog URL: https://github.com/Appegy/Union/releases

**CI Pipeline:**
- GitHub Actions (`.github/workflows/run-tests.yml`)
- Runs on `pull_request` and `workflow_dispatch`
- Builds and tests against .NET 9.0 on `ubuntu-latest`

## Roslyn Compiler Integration

This is the primary "integration" of the project. The source generator hooks into the C# compilation pipeline:

**Generator entry points:**
- `Appegy.Union.Generator~/Appegy.Union.Generator/Union/UnionAttributeGenerator.cs` - Implements `IIncrementalGenerator` for `[Union]` attribute
- `Appegy.Union.Generator~/Appegy.Union.Generator/Expose/ExposeAttributeGenerator.cs` - Implements `IIncrementalGenerator` for `[Expose]` attribute

**Analyzer entry points:**
- `Appegy.Union.Generator~/Appegy.Union.Generator/Union/UnionAttributeAnalyzer.cs` - Validates `[Union]` usage
- `Appegy.Union.Generator~/Appegy.Union.Generator/Expose/ExposeAttributeAnalyzer.cs` - Validates `[Expose]` usage

**How it integrates:**
- The generator DLL is referenced as an `Analyzer` in consuming projects (see `OutputItemType="Analyzer"` in test/shapes `.csproj` files)
- For Unity, the DLL ships in `Runtime/` alongside the assembly definition

## Unity Engine Integration

**Runtime attributes (consumed by Unity projects):**
- `Runtime/UnionAttribute.cs` - `[Union(typeof(A), typeof(B), ...)]` marks a partial struct/class as a discriminated union
- `Runtime/ExposeAttribute.cs` - `[Expose(typeof(IInterface))]` exposes interface members on the union type
- `Runtime/Appegy.Union.asmdef` - Unity assembly definition for the `Appegy.Union` namespace

**Unity test environment:**
- `Appegy.Union.Environment~/` - A full Unity project for in-editor testing
- Contains sample scripts at `Appegy.Union.Environment~/Assets/Scripts/`
- Hidden from Unity asset import via `~` suffix convention

## Environment Configuration

**Required env vars:**
- None

**Secrets location:**
- Not applicable

## Webhooks & Callbacks

**Incoming:**
- None

**Outgoing:**
- None

---

*Integration audit: 2026-03-09*
