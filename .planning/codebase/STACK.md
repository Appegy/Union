# Technology Stack

**Analysis Date:** 2026-03-09

## Languages

**Primary:**
- C# (latest LangVersion) - Source generator, runtime attributes, tests, shapes library

**Secondary:**
- None

## Runtime

**Environment:**
- .NET 9.0 - Used for tests, diagnostic tool, and shapes library
- .NET Standard 2.0 - Used for the source generator itself (`Appegy.Union.Generator.csproj`) to ensure broad Unity compatibility

**Package Manager:**
- NuGet (via `dotnet restore`)
- Lockfile: Not present (no `packages.lock.json` at solution level)

## Frameworks

**Core:**
- Microsoft.CodeAnalysis.CSharp 4.3.0 - Roslyn compiler APIs for C# source generation
- Microsoft.CodeAnalysis.Analyzers 3.3.3 - Analyzer infrastructure for the source generator

**Testing:**
- NUnit 3.13.3 - Unit test framework
- NUnit3TestAdapter 4.5.0 - Test adapter for `dotnet test`
- Microsoft.NET.Test.Sdk 17.13.0 - Test infrastructure

**Build/Dev:**
- MSBuild (via `dotnet build`) - Build system
- GitHub Actions - CI runner

## Target Platform

**Unity Game Engine:**
- This is a **Unity Package** (UPM) distributed via `package.json`
- Minimum Unity version: 2021.3
- Package name: `com.appegy.union`
- Version: 1.0.2
- License: MIT

**Unity Assembly Definition:**
- `Runtime/Appegy.Union.asmdef` - Defines the `Appegy.Union` assembly for Unity
- Root namespace: `Appegy.Union`
- No unsafe code allowed
- Auto-referenced: true

## Key Dependencies

**Critical:**
- Microsoft.CodeAnalysis.CSharp 4.3.0 - The entire project is a Roslyn source generator; this is the core dependency
- Microsoft.CodeAnalysis.Analyzers 3.3.3 - Required for analyzer rule enforcement (`EnforceExtendedAnalyzerRules`)

**Infrastructure:**
- No other external NuGet packages beyond Roslyn and test frameworks

## Project Structure (Solution)

**Solution file:** `Appegy.Union.Generator~/Appegy.Union.Generator.slnx`

**Projects:**

| Project | Target | Purpose |
|---------|--------|---------|
| `Appegy.Union.Generator` | netstandard2.0 | The Roslyn source generator (IIncrementalGenerator) |
| `Appegy.Union.Generator.Shapes` | net9.0 | Shape type definitions used as test inputs for source generation |
| `Appegy.Union.Generator.Tests` | net9.0 | NUnit tests validating generated code behavior |
| `Appegy.Union.Generator.Diagnostic` | net9.0 | Console app for debugging/diagnosing generator output |

## Configuration

**Environment:**
- No environment variables required
- No `.env` files present
- No secrets or API keys needed

**Build:**
- `Appegy.Union.Generator.csproj`: Contains a `CustomAfterBuild` target that copies the built DLL and PDB to `Runtime/` for Unity consumption
- Nullable reference types enabled across all projects
- The generator project uses `EnforceExtendedAnalyzerRules` for strict analyzer compliance

**Unity-specific conventions:**
- Directories ending with `~` (e.g., `Appegy.Union.Generator~`, `Appegy.Union.Environment~`) are hidden from Unity's Asset Database
- `.meta` files accompany each file/folder for Unity asset tracking

## Platform Requirements

**Development:**
- .NET 9.0 SDK (for building tests, shapes, diagnostic)
- .NET Standard 2.0 support (automatically included with .NET 9 SDK)
- Unity 2021.3+ (for testing the package in-editor via `Appegy.Union.Environment~`)

**Production:**
- The generator DLL (`Appegy.Union.Generator.dll`) ships pre-built in `Runtime/`
- End users only need Unity 2021.3+ with Roslyn source generator support
- No runtime .NET dependency beyond what Unity provides

## CI/CD

**GitHub Actions:**
- Workflow: `.github/workflows/run-tests.yml`
- Triggers: `pull_request`, `workflow_dispatch`
- Runner: `ubuntu-latest`
- .NET version: 9.0.x
- Steps: checkout, setup-dotnet, restore, build (Release), test

---

*Stack analysis: 2026-03-09*
