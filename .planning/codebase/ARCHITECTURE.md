# Architecture

**Analysis Date:** 2026-03-09

## Pattern Overview

**Overall:** Roslyn Incremental Source Generator with Composite Part-based Code Emission

**Key Characteristics:**
- C# Source Generator (`IIncrementalGenerator`) that runs at compile time inside the Roslyn compiler pipeline
- Composite pattern for code generation: each generated member (fields, constructors, equals, etc.) is a discrete `GeneratorPart<T>` composed into a tree
- Two parallel generator pipelines: `[Union]` attribute generates discriminated union structs/classes; `[Expose]` attribute generates interface delegation
- Companion `DiagnosticAnalyzer` classes provide compile-time validation with custom error codes (UNION002-UNION010)
- Ships as a Unity Package Manager (UPM) package (`com.appegy.union`) with the compiled generator DLL in `Runtime/`

## Layers

**Runtime Attributes (consumed by user code):**
- Purpose: Marker attributes that users apply to their partial structs/classes
- Location: `Runtime/`
- Contains: `UnionAttribute.cs`, `ExposeAttribute.cs`
- Depends on: Nothing (pure .NET `System.Attribute` subclasses)
- Used by: User code, Generator layer (by fully-qualified name match)

**Generator Core (Roslyn source generator):**
- Purpose: Roslyn `IIncrementalGenerator` implementations that produce source code at compile time
- Location: `Appegy.Union.Generator~/Appegy.Union.Generator/`
- Contains: Generators, analyzers, part-based code emitters, utilities
- Depends on: `Microsoft.CodeAnalysis.CSharp` 4.3.0
- Used by: Roslyn compiler pipeline (loaded as analyzer/generator)

**Test Shapes (generator integration test fixtures):**
- Purpose: Sample types that exercise the generator; used by both the Diagnostic runner and NUnit tests
- Location: `Appegy.Union.Generator~/Appegy.Union.Generator.Shapes/`
- Contains: `Shape` union definition, variant structs (`Circle`, `Rectangle`, `Hexagon`), attribute copies
- Depends on: Generator (as project analyzer reference)
- Used by: `Appegy.Union.Generator.Tests`, `Appegy.Union.Generator.Diagnostic`

**Test Suite:**
- Purpose: NUnit tests that validate generated code behavior (constructors, equality, properties, match, toString, hashCode)
- Location: `Appegy.Union.Generator~/Appegy.Union.Generator.Tests/`
- Contains: 7 test fixture classes covering all generated functionality
- Depends on: Shapes project, Generator (as analyzer), NUnit 3.13.3
- Used by: CI pipeline (`run-tests.yml`)

**Diagnostic Runner:**
- Purpose: Console app for debugging the generator/analyzer locally; loads Shapes source as embedded resources and runs both generators and analyzers programmatically
- Location: `Appegy.Union.Generator~/Appegy.Union.Generator.Diagnostic/`
- Contains: Single `Program.cs` entry point
- Depends on: Generator project (direct reference, not analyzer), Shapes source files (as embedded resources)
- Used by: Developer during debugging

**Unity Environment (sample project):**
- Purpose: Unity project for testing the package in a real Unity context
- Location: `Appegy.Union.Environment~/`
- Contains: Sample scripts mirroring the Shapes pattern, Unity project settings
- Depends on: Runtime package via UPM local reference
- Used by: Manual testing only

## Data Flow

**Union Code Generation Pipeline:**

1. User decorates a `partial struct` or `partial class` with `[Union(typeof(A), typeof(B), ...)]`
2. `UnionAttributeAnalyzer` validates: type must be `partial`, parent types must be `partial`, at least one type argument, no duplicates (diagnostic codes UNION002-004, UNION010)
3. `UnionAttributeGenerator.Initialize()` registers an incremental pipeline via `ForAttributeWithMetadataName("Appegy.Union.UnionAttribute")`
4. Transform extracts `TypeDeclarationSyntax`, resolves each type argument to `UnionTypeInfo` (name, full name, field name, param name), detects explicit `StructLayout`
5. `RegisterSourceOutput` streams `(syntax, types, explicitLayout)` to the emission callback
6. Emission callback creates an `IndentedTextWriter` and calls `AppendParts(Parts, input)` on the static part tree
7. Part tree emits: header -> parent scope (namespace/nesting) -> declaration (partial struct + IEquatable) -> scoped body { Kind enum, fields, properties, constructors, Match, ToString, GetHashCode, Equals, operators, comparison }
8. Output written as `{TypeName}_Union.g.cs`

**Expose Code Generation Pipeline:**

1. User decorates the same type with `[Expose(typeof(IFoo), ...)]`
2. `ExposeAttributeAnalyzer` validates: `[Union]` must also be present, arguments must be interfaces, no duplicates, all union variant types must implement all exposed interfaces (UNION005-009)
3. `ExposeAttributeGenerator.Initialize()` combines two incremental sources: expose attributes and union attributes
4. For each exposed type, resolves the list of union variant types and the list of interface types
5. Part tree emits: header -> parent scope -> declaration (partial struct : IFoo, IBar) -> scoped body { for each interface: properties, indexers, methods delegated via switch on `_type` }
6. Output written as `{TypeName}_Expose.g.cs`

**State Management:**
- No runtime state; all work happens at compile time
- Generated structs use a `Kind` enum (`_type` field) as discriminator and per-variant backing fields
- With `[StructLayout(LayoutKind.Explicit)]`, variant fields share `FieldOffset(1)` for memory-efficient overlapping

## Key Abstractions

**GeneratorPart<T>:**
- Purpose: Base class for all code emission units; each part writes one logical section of generated code
- Examples: `UnionFieldsPart`, `UnionConstructorsPart`, `UnionMatchPart`, `ImplementProperties`, `ImplementMethods`
- Pattern: Composite -- parts are composed into trees via `ScopedPart<T>` (wraps children in `{ }`) and `ParentScopedPart<T>` (wraps children in namespace/type nesting). `AppendParts()` extension iterates children with newline management.
- Location: `Appegy.Union.Generator~/Appegy.Union.Generator/Parts/GeneratorPart.cs`

**ScopedPart<T>:**
- Purpose: Wraps child parts inside curly braces with proper indentation
- Examples: Used to wrap the body of union declarations and expose declarations
- Pattern: Decorator over `IReadOnlyList<GeneratorPart<T>>`
- Location: `Appegy.Union.Generator~/Appegy.Union.Generator/Parts/ScopedPart.cs`

**ParentScopedPart<T>:**
- Purpose: Walks the syntax tree upward to reconstruct namespace and parent type nesting, wrapping child parts inside that scope
- Examples: `UnionParentScopedPart`, `ExposeParentScopedPart`
- Pattern: Template method -- subclasses provide `GetTypeSyntax(T)`, base handles scope walking
- Location: `Appegy.Union.Generator~/Appegy.Union.Generator/Parts/ParentScopedPart.cs`

**ExposeInterfacePart.Implementation:**
- Purpose: Strategy for generating a specific kind of interface member (property, indexer, method)
- Examples: `ImplementProperties`, `ImplementIndexers`, `ImplementMethods`
- Pattern: Chain of responsibility -- each implementation's `TryGenerateMember()` returns `true` if it handled the member
- Location: `Appegy.Union.Generator~/Appegy.Union.Generator/Expose/Parts/ExposeInterfacePart.cs`

**UnionAttributePartInput / ExposeAttributePartInput:**
- Purpose: Immutable data records carrying all information a part needs to generate code
- Pattern: Record structs passed through the part tree
- Location: `Appegy.Union.Generator~/Appegy.Union.Generator/Union/UnionAttributePartInput.cs`, `Appegy.Union.Generator~/Appegy.Union.Generator/Expose/ExposeAttributePartInput.cs`

**UnionTypeInfo / ExposeTypeInfo / ExposeInterfaceInfo:**
- Purpose: Precomputed type metadata (name, full name, field name, param name) to avoid repeated symbol resolution in parts
- Pattern: Readonly structs computed once during the transform phase
- Location: Same files as PartInput above

## Entry Points

**UnionAttributeGenerator:**
- Location: `Appegy.Union.Generator~/Appegy.Union.Generator/Union/UnionAttributeGenerator.cs`
- Triggers: Roslyn compiler detects `[Union]` attribute on a type declaration
- Responsibilities: Orchestrates the union generation pipeline, registers incremental source output

**ExposeAttributeGenerator:**
- Location: `Appegy.Union.Generator~/Appegy.Union.Generator/Expose/ExposeAttributeGenerator.cs`
- Triggers: Roslyn compiler detects `[Expose]` attribute on a type declaration
- Responsibilities: Orchestrates the expose generation pipeline, combines union type info with interface info

**UnionAttributeAnalyzer:**
- Location: `Appegy.Union.Generator~/Appegy.Union.Generator/Union/UnionAttributeAnalyzer.cs`
- Triggers: Roslyn compiler encounters an `[Attribute]` syntax node
- Responsibilities: Reports diagnostics UNION002-004, UNION010

**ExposeAttributeAnalyzer:**
- Location: `Appegy.Union.Generator~/Appegy.Union.Generator/Expose/ExposeAttributeAnalyzer.cs`
- Triggers: Roslyn compiler encounters an `[Attribute]` syntax node
- Responsibilities: Reports diagnostics UNION005-009

**Diagnostic Program (debug tool):**
- Location: `Appegy.Union.Generator~/Appegy.Union.Generator.Diagnostic/Program.cs`
- Triggers: Manual execution (`dotnet run`)
- Responsibilities: Loads Shapes source from embedded resources, runs analyzers and generators, prints diagnostics and generated code to console

## Error Handling

**Strategy:** Compile-time diagnostics via Roslyn `DiagnosticDescriptor` (no runtime error handling in the generator itself)

**Patterns:**
- Each analyzer validates preconditions and reports `Diagnostic` instances with specific IDs (UNION002-UNION010)
- Generators silently skip generation if preconditions fail (e.g., type not `partial`, no types provided) -- the analyzer reports the error separately
- Generated code throws `InvalidOperationException` for unreachable switch cases (unknown `Kind` value)
- Generated property getters throw `Exception` when accessing a variant that does not match the current `Kind`

**Diagnostic Codes:**
- `UNION002`: Type with `[Union]` is not `partial`
- `UNION003`: Nested parent type is not `partial`
- `UNION004`: No types provided in `[Union]`
- `UNION005`: Duplicate interface in `[Expose]`
- `UNION006`: Non-interface type in `[Expose]`
- `UNION007`: Union variant type does not implement exposed interface
- `UNION008`: `[Expose]` used without `[Union]`
- `UNION009`: No interfaces provided in `[Expose]`
- `UNION010`: Duplicate type in `[Union]`

## Cross-Cutting Concerns

**Logging:** Not applicable (compile-time tool; Diagnostic runner uses `Console.WriteLine`)
**Validation:** Roslyn `DiagnosticAnalyzer` pattern with `SyntaxNodeAction` registration
**Authentication:** Not applicable
**Code Output Formatting:** `System.CodeDom.Compiler.IndentedTextWriter` with 4-space indentation; `DisposableIndent` for scoped indent management; `NeedNewLine` property on parts controls blank line insertion between sections

---

*Architecture analysis: 2026-03-09*
