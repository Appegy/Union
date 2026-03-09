# Codebase Structure

**Analysis Date:** 2026-03-09

## Directory Layout

```
Union/
├── .claude/                        # Claude configuration
├── .github/
│   └── workflows/
│       └── run-tests.yml           # CI: runs NUnit tests
├── .planning/
│   └── codebase/                   # GSD analysis documents
├── Appegy.Union.Environment~/      # Unity sample project (excluded from UPM by ~ suffix)
│   ├── Assets/
│   │   ├── Editor/                 # Unity editor scripts
│   │   ├── Scenes/                 # Unity scenes
│   │   └── Scripts/                # Sample usage of Union in Unity
│   │       ├── Shape.cs            # Sample union struct
│   │       └── Variants/           # Sample variant types
│   ├── Packages/                   # Unity package manifest
│   └── ProjectSettings/            # Unity project settings
├── Appegy.Union.Generator~/        # Source generator solution (excluded from UPM by ~ suffix)
│   ├── Appegy.Union.Generator/     # Main generator project (netstandard2.0)
│   │   ├── Attributes/             # Attribute name constants
│   │   ├── Expose/                 # [Expose] generator + analyzer
│   │   │   ├── Implementations/    # Interface member generators (properties, indexers, methods)
│   │   │   └── Parts/              # Expose-specific code emission parts
│   │   ├── Parts/                  # Shared base parts (GeneratorPart, ScopedPart, etc.)
│   │   ├── Union/                  # [Union] generator + analyzer
│   │   │   └── Parts/              # Union-specific code emission parts
│   │   └── Utilities/              # Extension methods and helpers
│   ├── Appegy.Union.Generator.Diagnostic/  # Console debug runner
│   ├── Appegy.Union.Generator.Shapes/      # Test fixture types
│   │   └── Shapes/
│   │       ├── Attributes/         # Local copies of Union/Expose attributes
│   │       └── Variants/           # Circle, Rectangle, Hexagon structs
│   └── Appegy.Union.Generator.Tests/       # NUnit test project
│       └── Shapes/                 # Test fixtures
├── Runtime/                        # UPM package runtime (shipped to users)
│   ├── Appegy.Union.asmdef         # Unity assembly definition
│   ├── UnionAttribute.cs           # [Union] attribute source
│   ├── ExposeAttribute.cs          # [Expose] attribute source
│   ├── Appegy.Union.Generator.dll  # Compiled generator (auto-copied by build)
│   └── Appegy.Union.Generator.pdb  # Debug symbols
├── package.json                    # UPM package manifest
├── LICENSE                         # MIT license
└── README.md                       # Package documentation
```

## Directory Purposes

**`Runtime/`:**
- Purpose: The shipped UPM package content; contains attribute source files and the precompiled generator DLL
- Contains: `UnionAttribute.cs`, `ExposeAttribute.cs`, compiled `Appegy.Union.Generator.dll`, Unity `.asmdef`
- Key files: `Runtime/UnionAttribute.cs`, `Runtime/ExposeAttribute.cs`, `Runtime/Appegy.Union.Generator.dll`
- Note: The DLL is auto-copied here by the generator project's `CustomAfterBuild` MSBuild target

**`Appegy.Union.Generator~/Appegy.Union.Generator/`:**
- Purpose: The core Roslyn source generator and diagnostic analyzer implementation
- Contains: Two generator/analyzer pairs (Union + Expose), part-based code emission framework, utilities
- Key files: `Union/UnionAttributeGenerator.cs`, `Expose/ExposeAttributeGenerator.cs`, `DiagnosticDescriptors.cs`

**`Appegy.Union.Generator~/Appegy.Union.Generator/Parts/`:**
- Purpose: Shared base classes for the composite code emission pattern
- Contains: `GeneratorPart.cs` (abstract base), `ScopedPart.cs` (curly brace wrapper), `ParentScopedPart.cs` (namespace/nesting walker), `HeaderPart.cs` (auto-generated comment)

**`Appegy.Union.Generator~/Appegy.Union.Generator/Union/`:**
- Purpose: Everything related to `[Union]` attribute processing
- Contains: Generator, analyzer, input record, and 12 code emission parts
- Key files: `UnionAttributeGenerator.cs`, `UnionAttributeAnalyzer.cs`, `UnionAttributePartInput.cs`

**`Appegy.Union.Generator~/Appegy.Union.Generator/Union/Parts/`:**
- Purpose: Individual code sections generated for a union type
- Contains: `UnionTypeEnumPart` (Kind enum), `UnionFieldsPart`, `UnionPropertiesPart`, `UnionConstructorsPart`, `UnionMatchPart`, `UnionToStringPart`, `UnionGetHashCodePart`, `UnionEqualsPart`, `UnionOperatorsPart`, `UnionComparisonPart`, `UnionDeclarationPart`, `UnionParentScopedPart`

**`Appegy.Union.Generator~/Appegy.Union.Generator/Expose/`:**
- Purpose: Everything related to `[Expose]` attribute processing
- Contains: Generator, analyzer, input record, declaration/scope parts, and interface member implementations

**`Appegy.Union.Generator~/Appegy.Union.Generator/Expose/Implementations/`:**
- Purpose: Strategy classes for generating different kinds of interface members
- Contains: `ImplementProperties.cs`, `ImplementIndexers.cs`, `ImplementMethods.cs`

**`Appegy.Union.Generator~/Appegy.Union.Generator/Utilities/`:**
- Purpose: Shared extension methods for code writing and Roslyn syntax manipulation
- Contains: `CodeWriterExtensions.cs` (AppendParts, IndentScope, WriteFieldName, AppendFullTypeName), `SyntaxTreeExtensions.cs` (GetTypesFromConstructor, GetFieldName, GetParamName), `DisposableIndent.cs`

**`Appegy.Union.Generator~/Appegy.Union.Generator.Shapes/`:**
- Purpose: Test fixture types that the generator processes; shared by both Tests and Diagnostic projects
- Contains: `Shape` union definition, `IShape` interface, `Circle`/`Rectangle`/`Hexagon` variants, local attribute copies

**`Appegy.Union.Generator~/Appegy.Union.Generator.Tests/`:**
- Purpose: NUnit integration tests that validate generated code behavior at runtime
- Contains: 7 test fixture classes in `Shapes/` subdirectory

**`Appegy.Union.Generator~/Appegy.Union.Generator.Diagnostic/`:**
- Purpose: Console app for debugging generators/analyzers locally without a full compile
- Contains: Single `Program.cs` that programmatically invokes generators on embedded Shapes source

**`Appegy.Union.Environment~/`:**
- Purpose: Unity project for manual integration testing of the UPM package
- Contains: Sample scripts, Unity project configuration
- Note: The `~` suffix excludes this from Unity's package import

## Key File Locations

**Entry Points:**
- `Appegy.Union.Generator~/Appegy.Union.Generator/Union/UnionAttributeGenerator.cs`: Union source generator entry
- `Appegy.Union.Generator~/Appegy.Union.Generator/Expose/ExposeAttributeGenerator.cs`: Expose source generator entry
- `Appegy.Union.Generator~/Appegy.Union.Generator.Diagnostic/Program.cs`: Debug console app entry

**Configuration:**
- `package.json`: UPM package manifest (name, version, Unity compatibility)
- `Runtime/Appegy.Union.asmdef`: Unity assembly definition for the runtime
- `Appegy.Union.Generator~/Appegy.Union.Generator/Appegy.Union.Generator.csproj`: Generator build config (netstandard2.0, auto-copy DLL to Runtime/)
- `Appegy.Union.Generator~/Appegy.Union.Generator.Tests/Appegy.Union.Generator.Tests.csproj`: Test project config (NUnit, net9.0)

**Core Logic:**
- `Appegy.Union.Generator~/Appegy.Union.Generator/Parts/GeneratorPart.cs`: Abstract base for all code parts
- `Appegy.Union.Generator~/Appegy.Union.Generator/Parts/ParentScopedPart.cs`: Namespace/type nesting reconstruction
- `Appegy.Union.Generator~/Appegy.Union.Generator/DiagnosticDescriptors.cs`: All diagnostic error definitions
- `Appegy.Union.Generator~/Appegy.Union.Generator/Attributes/AttributesNames.cs`: Attribute FQN constants

**Testing:**
- `Appegy.Union.Generator~/Appegy.Union.Generator.Tests/Shapes/ShapeConstructorTests.cs`: Constructor tests
- `Appegy.Union.Generator~/Appegy.Union.Generator.Tests/Shapes/ShapeComparisonTests.cs`: Equality/comparison tests
- `Appegy.Union.Generator~/Appegy.Union.Generator.Tests/Shapes/ShapePropertyTests.cs`: Property access tests
- `Appegy.Union.Generator~/Appegy.Union.Generator.Tests/Shapes/ShapeInterfaceAreaTests.cs`: Expose/interface delegation tests

**Runtime Attributes:**
- `Runtime/UnionAttribute.cs`: `[Union(params Type[])]` attribute
- `Runtime/ExposeAttribute.cs`: `[Expose(params Type[])]` attribute

## Naming Conventions

**Files:**
- Generator parts: `Union{Feature}Part.cs` or `Expose{Feature}Part.cs` (e.g., `UnionFieldsPart.cs`, `ExposeDeclarationPart.cs`)
- Analyzers: `{Attribute}Analyzer.cs` (e.g., `UnionAttributeAnalyzer.cs`)
- Generators: `{Attribute}Generator.cs` (e.g., `UnionAttributeGenerator.cs`)
- Input records: `{Attribute}PartInput.cs` (e.g., `UnionAttributePartInput.cs`)
- Implementations: `Implement{MemberType}.cs` (e.g., `ImplementProperties.cs`)
- Tests: `Shape{Feature}Tests.cs` (e.g., `ShapeConstructorTests.cs`)

**Directories:**
- `Parts/` for code emission components
- `Implementations/` for strategy implementations
- `Utilities/` for extension methods and helpers
- `Shapes/` and `Variants/` for test fixture types
- `~` suffix on directories excluded from Unity package import

**Projects:**
- `Appegy.Union.Generator` -- main generator
- `Appegy.Union.Generator.{Purpose}` -- auxiliary projects (Diagnostic, Shapes, Tests)

## Where to Add New Code

**New Union Part (e.g., generating a new member on union types):**
- Create: `Appegy.Union.Generator~/Appegy.Union.Generator/Union/Parts/Union{Feature}Part.cs`
- Extend `GeneratorPart<UnionAttributePartInput>`
- Register in the `Parts` list in `Appegy.Union.Generator~/Appegy.Union.Generator/Union/UnionAttributeGenerator.cs`

**New Expose Implementation (e.g., supporting events on interfaces):**
- Create: `Appegy.Union.Generator~/Appegy.Union.Generator/Expose/Implementations/Implement{MemberType}.cs`
- Extend `ExposeInterfacePart.Implementation`
- Register in the `Parts` list in `Appegy.Union.Generator~/Appegy.Union.Generator/Expose/ExposeAttributeGenerator.cs`

**New Diagnostic Rule:**
- Add descriptor to `Appegy.Union.Generator~/Appegy.Union.Generator/DiagnosticDescriptors.cs`
- Add validation method to relevant analyzer (`UnionAttributeAnalyzer.cs` or `ExposeAttributeAnalyzer.cs`)
- Register descriptor in the analyzer's `SupportedDiagnostics` array

**New Test:**
- Create: `Appegy.Union.Generator~/Appegy.Union.Generator.Tests/Shapes/Shape{Feature}Tests.cs`
- Use NUnit `[TestFixture]` and `[Test]` attributes
- If new variant types are needed, add to `Appegy.Union.Generator~/Appegy.Union.Generator.Shapes/Shapes/Variants/`

**New Utility Extension:**
- Add to `Appegy.Union.Generator~/Appegy.Union.Generator/Utilities/CodeWriterExtensions.cs` (for code writing) or `SyntaxTreeExtensions.cs` (for Roslyn helpers)

**New Runtime Attribute:**
- Add `.cs` file to `Runtime/`
- Add corresponding FQN constant to `Appegy.Union.Generator~/Appegy.Union.Generator/Attributes/AttributesNames.cs`

## Special Directories

**`Runtime/`:**
- Purpose: UPM package content shipped to Unity users
- Generated: Partially -- `Appegy.Union.Generator.dll` and `.pdb` are copied here by MSBuild `CustomAfterBuild` target
- Committed: Yes (DLL is committed so Unity users get the generator without building)

**`Appegy.Union.Environment~/`:**
- Purpose: Unity sample/test project
- Generated: No
- Committed: Yes, but excluded from UPM package import by `~` suffix convention

**`Appegy.Union.Generator~/`:**
- Purpose: Generator solution root
- Generated: No (except `bin/` and `obj/` under each project)
- Committed: Source files yes; `bin/`/`obj/` should not be (the DLL is copied to `Runtime/` instead)

---

*Structure analysis: 2026-03-09*
