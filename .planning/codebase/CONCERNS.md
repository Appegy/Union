# Codebase Concerns

**Analysis Date:** 2026-03-09

## Tech Debt

**Default union state is ambiguous (zero-initialized Kind enum):**
- Issue: The generated `Kind` enum starts at 0, meaning the first variant type is the default value for an uninitialized union struct. A `default(Shape)` or `new Shape()` will have `_type == Kind.Circle` (the first variant) but all fields at their defaults, creating a silently invalid state that appears valid.
- Files: `Appegy.Union.Generator~/Appegy.Union.Generator/Union/Parts/UnionTypeEnumPart.cs` (line 16-19), `Appegy.Union.Generator~/Appegy.Union.Generator/Union/Parts/UnionFieldsPart.cs`
- Impact: Users may accidentally use a default-constructed union and get misleading behavior. Accessing the first variant's property will return a default value instead of throwing, unlike accessing any other variant which would throw. The `ToString()`, `GetHashCode()`, and `Equals()` methods will silently operate on the default-valued first variant.
- Fix approach: Start the `Kind` enum at 1 (or add a `None = 0` sentinel value) and add a guard in property getters / `Match` / `ToString` / `GetHashCode` to throw `InvalidOperationException` for the uninitialized state.

**Property setter does not clear other variant fields:**
- Issue: When a user sets a variant property (e.g., `shape.Rectangle = new Rectangle(...)`) the generated setter updates `_type` and the target field but does NOT zero out the previously active variant field. Without `[StructLayout(LayoutKind.Explicit)]`, stale data remains in the other field(s).
- Files: `Appegy.Union.Generator~/Appegy.Union.Generator/Union/Parts/UnionPropertiesPart.cs` (lines 32-39)
- Impact: For non-explicit-layout structs, this is wasted memory and potential confusion during debugging. For explicit-layout structs, the fields overlap so it is not an issue. The constructors correctly set other fields to `default` (see `UnionConstructorsPart.cs` lines 28-36), but property setters do not follow this pattern.
- Fix approach: Generate `otherField = default;` assignments in property setters for non-explicit-layout unions, matching the constructor pattern.

**Equals(object) throws on non-matching types instead of returning false:**
- Issue: The generated `Equals(object boxed)` override uses a switch expression with a default arm that throws `InvalidOperationException`. Per .NET conventions, `Equals(object)` should return `false` for unrecognized types, never throw.
- Files: `Appegy.Union.Generator~/Appegy.Union.Generator/Union/Parts/UnionEqualsPart.cs` (line 22)
- Impact: Comparing a union with an unrelated object (e.g., `shape.Equals("hello")` or `shape.Equals(42)`) throws at runtime instead of returning `false`. This violates the `System.Object.Equals` contract and will break any collection or framework code that calls `Equals` polymorphically.
- Fix approach: Change the default switch arm from `throw` to `=> false`.

**Missing diagnostic ID UNION001:**
- Issue: Diagnostic IDs start at UNION002. UNION001 is skipped with no comment explaining why.
- Files: `Appegy.Union.Generator~/Appegy.Union.Generator/DiagnosticDescriptors.cs`
- Impact: Minor inconsistency. Could confuse contributors looking for the first diagnostic.
- Fix approach: Either add a comment explaining why UNION001 is reserved/removed, or renumber diagnostics.

**Duplicate type-name collision potential in generated code:**
- Issue: The generated `Kind` enum uses the simple `Name` of each variant type. If two variant types from different namespaces share the same simple name, the generated enum will have duplicate members and the code will not compile. The analyzer (`VerifyNoDuplicate`) checks for duplicates by fully-qualified name, so it would not flag `Foo.Circle` and `Bar.Circle` as duplicates even though both generate `Kind.Circle`.
- Files: `Appegy.Union.Generator~/Appegy.Union.Generator/Union/Parts/UnionTypeEnumPart.cs`, `Appegy.Union.Generator~/Appegy.Union.Generator/Union/UnionAttributeAnalyzer.cs` (line 117)
- Impact: Compilation failure with a confusing error when variant types from different namespaces share a name.
- Fix approach: Add an analyzer diagnostic that detects simple-name collisions, or qualify enum member names when collisions exist.

**Field name collision with single-character type names:**
- Issue: `GetFieldName` in `SyntaxTreeExtensions.cs` generates field names as `_` + lowercased first char + rest of name. For a type named `A`, the field becomes `_a`. If the union type itself or any user field is named `_a`, there is a collision. No collision check exists.
- Files: `Appegy.Union.Generator~/Appegy.Union.Generator/Utilities/SyntaxTreeExtensions.cs` (lines 30-38)
- Impact: Rare but possible compilation failure due to duplicate field names in generated partial struct.
- Fix approach: Add a collision detection step or use a more distinctive prefix (e.g., `__union_`).

## Known Bugs

**Implicit conversion operator throws on wrong-type access:**
- Symptoms: `Shape shape = new Shape(new Rectangle(4, 6)); Circle c = shape;` throws `System.Exception` at runtime because the implicit operator calls the variant property getter which throws.
- Files: `Appegy.Union.Generator~/Appegy.Union.Generator/Union/Parts/UnionOperatorsPart.cs` (lines 17-23)
- Trigger: Using implicit conversion to extract a variant type that is not the currently active variant.
- Workaround: Check `shape.Type` before converting. However, the implicit conversion operator suggests the conversion is always safe, which is misleading. Consider removing the implicit conversion from union-to-variant or making it return a nullable/use `TryGet` pattern.

**FieldOffset(1) hardcoded for explicit layout assumes Kind is 1 byte:**
- Symptoms: The generated code uses `[FieldOffset(0)]` for `_type` (a `Kind` enum backed by `System.Byte`) and `[FieldOffset(1)]` for all variant fields. This is correct only because `Kind` is `Byte`. If the enum backing type ever changes, the offset becomes wrong.
- Files: `Appegy.Union.Generator~/Appegy.Union.Generator/Union/Parts/UnionFieldsPart.cs` (lines 13-14, 20-22)
- Trigger: Changing `Kind` enum to a larger backing type, or if a future version needs more than 255 variants.
- Workaround: None needed currently (Byte supports 256 variants). Document the coupling or derive the offset from the enum backing size.

## Security Considerations

**No significant security risks identified:**
- Risk: This is a compile-time source generator for Unity; it does not handle user input, network traffic, or sensitive data at runtime.
- Current mitigation: The generator runs only at compile time within the Roslyn pipeline.
- Recommendations: None critical. Ensure the generator DLL distributed in `Runtime/` is built from the same source (the `CustomAfterBuild` target copies DLLs automatically).

## Performance Bottlenecks

**String concatenation in code generation:**
- Problem: The generator uses `IndentedTextWriter` with many small `Write` calls, which is reasonable. However, `ExposeAttributeGenerator` uses `.Collect()` to gather all union sources into an `ImmutableArray` and then creates a `Dictionary` for each expose source via `.Select()`. This means the dictionary is rebuilt for every `[Expose]`-annotated type.
- Files: `Appegy.Union.Generator~/Appegy.Union.Generator/Expose/ExposeAttributeGenerator.cs` (lines 52-68, 72-81)
- Cause: The `Combine` + `Select` pattern rebuilds the `unionMap` dictionary every time any expose source changes. For projects with many union types, this is redundant work.
- Improvement path: Use a `Collect()` on expose sources too and process them in a single `RegisterSourceOutput` with the combined data, or cache the dictionary construction.

**No incremental caching of UnionTypeInfo/ExposeTypeInfo:**
- Problem: `UnionAttributePartInput` holds `TypeDeclarationSyntax` which is not equatable by value, meaning the incremental generator cache cannot skip re-generation when the syntax tree is unchanged but rebuilt.
- Files: `Appegy.Union.Generator~/Appegy.Union.Generator/Union/UnionAttributePartInput.cs`, `Appegy.Union.Generator~/Appegy.Union.Generator/Union/UnionAttributeGenerator.cs` (lines 40-58)
- Cause: `record struct UnionAttributePartInput` uses default equality, but `TypeDeclarationSyntax` reference equality changes on every compilation. The incremental pipeline will re-run code generation even when the input is semantically identical.
- Improvement path: Extract only the serializable/equatable data needed for generation (type name, namespace, list of variant names and fully-qualified names) into a plain record struct, and pass that instead of the raw `TypeDeclarationSyntax`.

## Fragile Areas

**Code generation via manual string writing:**
- Files: All files in `Appegy.Union.Generator~/Appegy.Union.Generator/Union/Parts/` and `Appegy.Union.Generator~/Appegy.Union.Generator/Expose/Implementations/`
- Why fragile: Every generated code feature is built by manually calling `codeWriter.Write()` and `codeWriter.WriteLine()` with careful indentation tracking. A single missed `Indent++`/`Indent--` or forgotten `codeWriter.WriteLine(';')` produces broken generated code that is hard to debug.
- Safe modification: When modifying generation parts, always build the test project (`Appegy.Union.Generator.Tests`) and inspect the generated output via the Diagnostic project. Add new test cases for any new generated code patterns.
- Test coverage: The test suite (`Appegy.Union.Generator.Tests/Shapes/`) covers runtime behavior of generated code but does NOT verify the generated source text itself (no snapshot/approval tests).

**ExposeAttributeGenerator coupling to UnionAttributeGenerator:**
- Files: `Appegy.Union.Generator~/Appegy.Union.Generator/Expose/ExposeAttributeGenerator.cs` (lines 52-68)
- Why fragile: `ExposeAttributeGenerator` independently re-parses `[Union]` attributes to find variant types. If the `UnionAttributeGenerator`'s parsing logic diverges (e.g., handling new attribute parameters), the Expose generator may produce inconsistent results.
- Safe modification: Keep both generators' attribute parsing in sync. Consider extracting shared parsing logic into a common utility.
- Test coverage: `ShapeInterfaceAreaTests.cs` covers the Expose feature but only for one interface with one property and one void method.

## Scaling Limits

**Kind enum limited to 256 variants:**
- Current capacity: 256 variant types per union (backed by `System.Byte`).
- Limit: More than 255 named variants would overflow. The enum also has no sentinel for "uninitialized."
- Scaling path: Change the backing type to `ushort` or `int` if needed. This would require updating the `[FieldOffset(1)]` hardcoded value for explicit layout unions.

## Dependencies at Risk

**Microsoft.CodeAnalysis.CSharp pinned to 4.3.0:**
- Risk: Version 4.3.0 targets .NET Standard 2.0 for Unity compatibility, but it is significantly behind current Roslyn versions (4.12+). Newer C# language features in user code (e.g., primary constructors, collection expressions) may produce syntax nodes that this version does not fully support.
- Impact: Users writing variant types with newer C# syntax may encounter unexpected generator behavior or silent failures.
- Migration plan: Upgrade cautiously; the generator must target `netstandard2.0` for Unity compatibility. Test with the minimum Unity-supported Roslyn version.

## Missing Critical Features

**No `Match<TResult>` overload (returning a value):**
- Problem: The generated `Match` method only supports `Action<T>` delegates (void returns). There is no `Match<TResult>` that accepts `Func<T, TResult>` delegates and returns a value.
- Blocks: Users cannot use functional-style pattern matching to transform union values without introducing a mutable local variable.

**No `TryGet` pattern:**
- Problem: There is no generated `bool TryGetCircle(out Circle value)` style accessor. Users must check `Type` and then access the property, or catch the exception from wrong-type access.
- Blocks: Safe extraction patterns without exception handling.

**No support for generic variant types:**
- Problem: If a user passes a generic type as a union variant (e.g., `[Union(typeof(Option<int>))]`), the generated field names and enum member names are derived from `Symbol.Name` which would be `Option` without the generic arity, potentially colliding with other `Option<T>` instantiations.
- Blocks: Using generic structs as union variants.

**No serialization support:**
- Problem: While the `Kind` enum is marked `[Serializable]`, the union struct itself has no serialization attributes or custom serialization logic. Unity's serialization system will not correctly serialize/deserialize union types.
- Blocks: Persisting union values in Unity scenes, prefabs, or ScriptableObjects.

## Test Coverage Gaps

**No tests for error/exception paths:**
- What's not tested: Accessing a variant property when the wrong type is active (should throw), calling `Match` on a default-constructed union, `Equals(object)` with non-union types.
- Files: `Appegy.Union.Generator.Tests/Shapes/` - all test files only cover happy paths.
- Risk: The exception-throwing behavior documented in generated code is unverified. The `Equals(object)` bug (throwing instead of returning false) would have been caught.
- Priority: High

**No analyzer diagnostic tests:**
- What's not tested: None of the analyzer diagnostics (UNION002 through UNION010) are tested. There are no tests verifying that invalid usage produces the expected diagnostic.
- Files: `Appegy.Union.Generator~/Appegy.Union.Generator/Union/UnionAttributeAnalyzer.cs`, `Appegy.Union.Generator~/Appegy.Union.Generator/Expose/ExposeAttributeAnalyzer.cs`
- Risk: Analyzer regressions (false positives or missed diagnostics) would go unnoticed.
- Priority: High

**No tests for class unions:**
- What's not tested: The generator supports `class` unions (commit `964e85c` "Allow class unions") but all tests use `struct` shapes. No tests verify generated code for `partial class` unions.
- Files: `Appegy.Union.Generator~/Appegy.Union.Generator/Union/Parts/UnionDeclarationPart.cs` (lines 16-23), `Appegy.Union.Generator~/Appegy.Union.Generator/Expose/Parts/ExposeDeclarationPart.cs` (lines 14-22)
- Risk: Class union generation may have bugs not caught by struct-only tests. Class unions have different semantics (null references, boxing behavior).
- Priority: Medium

**No tests for explicit struct layout vs automatic layout:**
- What's not tested: The explicit layout path (`[StructLayout(LayoutKind.Explicit)]`) generates `[FieldOffset]` attributes. The test `Shape` uses explicit layout, but there is no companion test with automatic layout to verify the non-explicit code path.
- Files: `Appegy.Union.Generator~/Appegy.Union.Generator/Union/Parts/UnionFieldsPart.cs`
- Risk: The automatic layout path (without `[FieldOffset]` attributes) is untested.
- Priority: Medium

**No generated source snapshot tests:**
- What's not tested: The actual text of generated `.g.cs` files is never verified. Tests only exercise runtime behavior of generated code.
- Files: All files in `Appegy.Union.Generator~/Appegy.Union.Generator/Union/Parts/` and `Appegy.Union.Generator~/Appegy.Union.Generator/Expose/`
- Risk: Generated code formatting or structure could regress without detection. Subtle issues (missing semicolons, wrong indentation) would only surface as compilation errors.
- Priority: Medium

---

*Concerns audit: 2026-03-09*
