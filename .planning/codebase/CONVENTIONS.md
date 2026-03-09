# Coding Conventions

**Analysis Date:** 2026-03-09

## Naming Patterns

**Files:**
- Use PascalCase for all C# file names: `UnionAttributeGenerator.cs`, `CodeWriterExtensions.cs`
- One primary type per file; file name matches the type name
- Test files use `{Subject}{Aspect}Tests.cs` pattern: `ShapeComparisonTests.cs`, `ShapeConstructorTests.cs`

**Classes/Structs:**
- PascalCase for all types: `UnionAttributeGenerator`, `ExposeInterfacePart`, `DisposableIndent`
- Generator part classes use `Union{Concern}Part` or `Expose{Concern}Part` naming: `UnionFieldsPart`, `UnionEqualsPart`, `ExposeDeclarationPart`
- Analyzer classes use `{Attribute}Analyzer` naming: `UnionAttributeAnalyzer`, `ExposeAttributeAnalyzer`
- Input data types use `{Feature}PartInput` naming: `UnionAttributePartInput`, `ExposeAttributePartInput`

**Functions/Methods:**
- PascalCase for public/private methods: `Generate()`, `Initialize()`, `HasExplicitStructLayoutAttribute()`
- Private methods often use `Verify{What}` for validation: `VerifyPartialModifier()`, `VerifyParentsPartial()`, `VerifyNoDuplicate()`
- Code generation methods use `Generate{What}` pattern: `GenerateProperty()`, `GenerateGetterBody()`, `GenerateMethodHeader()`
- Local functions use camelCase (enforced by `.editorconfig`): `addConstraint()` in `ImplementMethods.cs`

**Variables:**
- camelCase for local variables: `codeWriter`, `attributeSyntax`, `ancestorCount`
- Private fields use `_camelCase` prefix: `_type`, `_codeWriter`, `_isDisposed`
- Prefer `var` for all local variable declarations (enforced by `.editorconfig`)

**Constants:**
- Private constants use PascalCase: `Category` in `DiagnosticDescriptors.cs`
- Public constants use PascalCase: `UnionAttributeName`, `ExposeAttributeName`

**Types/Generics:**
- Single-letter generic parameter `T` for generic base classes: `GeneratorPart<T>`, `ScopedPart<T>`
- Record structs for immutable data transfer: `record struct UnionAttributePartInput`
- Readonly structs for value types: `readonly struct UnionTypeInfo`, `readonly struct Circle`

## Code Style

**Formatting:**
- `.editorconfig` enforced (see `D:/Development/Repos/_packages/Union/.editorconfig`)
- 4 spaces indentation for C# files
- UTF-8 charset, LF line endings
- No final newline (`insert_final_newline=false`)
- Trim trailing whitespace
- Max line length: 200 characters (ReSharper setting)

**Linting:**
- ReSharper configured via `.editorconfig`
- `EnforceExtendedAnalyzerRules=true` in generator project
- Nullable reference types enabled (`<Nullable>enable</Nullable>`)
- No `this.` qualifier (enforced by `.editorconfig`)
- Predefined types preferred for locals/parameters (e.g., `string` not `String`)

**Key Style Rules:**
- Prefer `var` everywhere (built-in types, apparent types, and elsewhere)
- No redundant parentheses in binary operators
- Modifier order: `public, private, protected, internal, new, abstract, virtual, sealed, override, static, readonly, extern, unsafe, volatile, async`
- Accessibility modifiers required for non-interface members

## Import Organization

**Order:**
1. `System.*` namespaces
2. `Microsoft.*` namespaces (Roslyn APIs)
3. Project-internal namespaces (`Appegy.Union.Generator`)

**Style:**
- File-scoped namespaces for generator code: `namespace Appegy.Union.Generator;`
- Block-scoped namespaces for Runtime attributes (Unity compatibility): `namespace Appegy.Union { ... }`
- `using static` for frequently referenced constant collections: `using static Appegy.Union.Generator.AttributesNames;`, `using static Appegy.Union.Generator.DiagnosticDescriptors;`

**Path Aliases:**
- None; all imports use full namespace paths

## Error Handling

**Generated Code Errors:**
- Generated switch statements always include `default: throw new global::System.InvalidOperationException(...)` for exhaustiveness
- Property getters on wrong union type throw `Exception` (via generated code)
- All generated type references use `global::` prefix to avoid namespace conflicts

**Analyzer Diagnostics:**
- Use `DiagnosticDescriptor` constants defined in `Appegy.Union.Generator~/Appegy.Union.Generator/DiagnosticDescriptors.cs`
- Diagnostic IDs follow `UNION{NNN}` pattern starting at UNION002
- All diagnostics are errors (`DiagnosticSeverity.Error`), enabled by default
- Category is always `"Union"`

**Validation Pattern in Analyzers:**
- Early return on non-matching conditions (guard clause pattern)
- Create `Diagnostic` via `Diagnostic.Create()` with descriptor, location, and format args
- Report via `context.ReportDiagnostic(diagnostic)`

## Logging

**Framework:** None (this is a source generator library; no runtime logging)

**Generated Code:** Uses `Console.WriteLine` in test shape types for demonstration only (see `Circle.LogArea()` in `Appegy.Union.Generator~/Appegy.Union.Generator.Shapes/Shapes/Variants/Circle.cs`)

## Comments

**When to Comment:**
- Comments are rare in this codebase; code is self-documenting
- No XML doc comments on any public APIs
- `// <auto-generated/>` header is added to all generated source files via `HeaderPart`

**JSDoc/TSDoc:**
- Not applicable (C# project)

## Function Design

**Size:** Methods are kept small and focused. Generator parts typically have a single `Generate()` method that delegates to private static helpers for sub-concerns.

**Parameters:**
- Use primary constructors for simple data types: `readonly struct Circle(double radius)`
- Use primary constructors for dependency injection in parts: `ScopedPart(IReadOnlyList<GeneratorPart<T>> innerParts)`
- `IndentedTextWriter codeWriter` is the first parameter in all code generation methods

**Return Values:**
- `bool` return for `TryGenerate*` pattern methods (see `ImplementProperties.TryGenerateMember()`)
- Void returns for `Generate()` methods that write to `IndentedTextWriter`

## Module Design

**Exports:**
- Public classes for generator parts, analyzers, and generators
- `internal static` for utility/extension classes: `CodeWriterExtensions`, `SyntaxTreeExtensions`
- `public static` for constants: `AttributesNames`, `DiagnosticDescriptors`

**Barrel Files:**
- Not used; each file contains one type

## Architecture Patterns

**Composite Pattern for Code Generation:**
- `GeneratorPart<T>` is the abstract base (see `Appegy.Union.Generator~/Appegy.Union.Generator/Parts/GeneratorPart.cs`)
- `ScopedPart<T>` wraps inner parts in `{ }` braces with indentation (see `Appegy.Union.Generator~/Appegy.Union.Generator/Parts/ScopedPart.cs`)
- `ParentScopedPart<T>` handles namespace/type nesting (see `Appegy.Union.Generator~/Appegy.Union.Generator/Parts/ParentScopedPart.cs`)
- Parts are composed as a tree in the generator's static `Parts` property

**Incremental Generator Pattern:**
- Both generators implement `IIncrementalGenerator`
- Use `ForAttributeWithMetadataName` for efficient attribute-driven generation
- Use `static` lambdas consistently in pipeline callbacks to avoid closures
- Guard clauses at start of `RegisterSourceOutput` callback (check for `partial` keyword, non-empty type lists)

**Struct-Based Inputs:**
- All generator part inputs are `struct` types (required by `GeneratorPart<T> where T : struct`)
- `record struct` used for inputs needing value equality: `UnionAttributePartInput`
- `readonly struct` with explicit fields for types needing computed properties: `UnionTypeInfo`

**IDisposable for Scope Management:**
- `DisposableIndent` manages `IndentedTextWriter.Indent++/--` via `using` blocks
- Used in complex generation methods like `UnionMatchPart.Generate()`

---

*Convention analysis: 2026-03-09*
