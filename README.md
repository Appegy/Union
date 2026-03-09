# Union

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![Unity 2021.3+](https://img.shields.io/badge/Unity-2021.3%2B-black.svg)](https://unity.com/)
[![.NET Tests](https://github.com/Appegy/Union/actions/workflows/run-tests.yml/badge.svg)](https://github.com/Appegy/Union/actions)

A C# source generator that creates **discriminated union types** -- type-safe containers that hold exactly one of several specified types at a time. Works with both `struct` and `class` variants. Comes with automatic interface delegation via `[Expose]`.

Built as a **Unity Package Manager (UPM)** package, but the generated code is pure C# with no Unity dependencies.

## Table of Contents

- [Installation](#installation)
- [Quick Start](#quick-start)
- [Union Attribute](#union-attribute)
  - [Generated Members](#generated-members)
  - [Memory Layout](#memory-layout)
  - [Pattern Matching](#pattern-matching)
  - [Equality and Comparison](#equality-and-comparison)
  - [Implicit Conversions](#implicit-conversions)
- [Expose Attribute](#expose-attribute)
  - [Interface Member Support Matrix](#interface-member-support-matrix)
  - [C# Version Compatibility](#c-version-compatibility)
- [Diagnostics](#diagnostics)
- [Limitations](#limitations)
- [License](#license)

## Installation

Add the package via Unity Package Manager using the git URL:

```
https://github.com/Appegy/Union.git
```

Or add directly to `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.appegy.union": "https://github.com/Appegy/Union.git"
  }
}
```

**Requirements:** Unity 2021.3 or later (requires Roslyn source generator support).

## Quick Start

1. Define your variant structs and a shared interface:

```csharp
public interface IShape
{
    double Area { get; }
}

public struct Circle : IShape
{
    public double Radius;
    public double Area => Math.PI * Radius * Radius;
}

public struct Rectangle : IShape
{
    public double Width, Height;
    public double Area => Width * Height;
}
```

2. Declare the union with `[Union]` and optionally `[Expose]`:

```csharp
[Union(typeof(Circle), typeof(Rectangle))]
[Expose(typeof(IShape))]
[StructLayout(LayoutKind.Explicit, Pack = 1)]
public partial struct Shape { }
```

3. Use it:

```csharp
Shape shape = new Circle { Radius = 5 };

// Access via exposed interface
Console.WriteLine(shape.Area); // 78.54

// Type-safe switching
switch (shape.Type)
{
    case Shape.Kind.Circle:    Console.WriteLine(shape.Circle.Radius); break;
    case Shape.Kind.Rectangle: Console.WriteLine(shape.Rectangle.Width); break;
}

// Pattern matching with Match
shape.Match(
    circle    => Console.WriteLine($"Circle r={circle.Radius}"),
    rectangle => Console.WriteLine($"Rect {rectangle.Width}x{rectangle.Height}")
);
```

## Union Attribute

`[Union(typeof(A), typeof(B), ...)]` is placed on a `partial struct` or `partial class`. Variant types can also be structs or classes. The source generator produces all the boilerplate for a type-safe tagged union.

```csharp
// Struct union (recommended for value-type variants)
[Union(typeof(VoidCell), typeof(EmptyCell), typeof(RegularCell))]
public partial struct PuzzleCell { }

// Class union (when you need reference semantics or class variants)
[Union(typeof(Dog), typeof(Cat))]
public partial class Animal { }
```

The union type can be nested inside other types -- the generator reconstructs the full namespace and type nesting automatically.

### Generated Members

For a union with variants `A`, `B`, `C`, the generator produces:

| Member | Description |
|--------|-------------|
| `enum Kind : byte` | Discriminator with one entry per variant (`Kind.A`, `Kind.B`, `Kind.C`). Marked `[Serializable]`. |
| `Kind Type { get; }` | Read-only property returning the currently held variant kind. |
| `A A { get; set; }` | Typed accessor for each variant. Getter throws if the wrong variant is active. Setter switches the active variant. |
| `PuzzleCell(A value)` | Constructor for each variant type. |
| `void Match(Action<A>, Action<B>, ...)` | Exhaustive pattern matching -- exactly one callback is invoked. |
| `ToString()` | Delegates to the active variant's `ToString()`. |
| `GetHashCode()` | Delegates to the active variant's `GetHashCode()`. |
| `Equals(object)` | Supports comparison with union instances and raw variant values. |
| `Equals(PuzzleCell)` | Typed equality: same `Kind` and equal variant values. |
| `Equals(A)` | Direct comparison with each variant type. |
| `==`, `!=` | Operators for union-to-union and union-to-variant comparisons. |
| `implicit operator` | Implicit conversion both ways: variant to union and union to variant. |
| `IEquatable<PuzzleCell>` | Implements `IEquatable<T>` for the union and each variant type. |

### Memory Layout

When you add `[StructLayout(LayoutKind.Explicit, Pack = 1)]` to a **struct** union, variant fields overlap in memory:

```
[FieldOffset(0)] Kind _type     (1 byte)
[FieldOffset(1)] A    _a        ┐
[FieldOffset(1)] B    _b        ├─ overlapping (largest variant wins)
[FieldOffset(1)] C    _c        ┘
```

Total size = 1 byte + size of the largest variant. This is the recommended mode for struct unions with struct variants.

Without explicit layout (or for class unions), fields are laid out sequentially (no overlap), and total size = 1 byte + sum of all variant sizes.

> **Note:** `[StructLayout(LayoutKind.Explicit)]` only has effect on structs. For class unions, fields are always sequential.

### Pattern Matching

The `Match` method provides exhaustive pattern matching:

```csharp
shape.Match(
    circle    => Console.WriteLine($"Circle with radius {circle.Radius}"),
    rectangle => Console.WriteLine($"Rectangle {rectangle.Width}x{rectangle.Height}"),
    hexagon   => Console.WriteLine($"Hexagon with side {hexagon.SideLength}")
);
```

Every variant must have a handler -- the compiler enforces this at the call site.

### Equality and Comparison

Two union values are equal when they hold the same `Kind` and the variant values are equal:

```csharp
var a = new Shape(new Circle(5));
var b = new Shape(new Circle(5));
var c = new Shape(new Circle(3));

a == b  // true  -- same Kind, same value
a == c  // false -- same Kind, different value
a == new Circle(5)  // true -- direct variant comparison
```

### Implicit Conversions

Variants convert implicitly to and from the union:

```csharp
// Variant -> Union (always safe)
Shape shape = new Circle(5);

// Union -> Variant (throws if wrong Kind!)
Circle circle = shape;  // OK if shape holds Circle
Rectangle rect = shape; // throws Exception
```

> **Warning:** The union-to-variant conversion throws if the active variant does not match. Check `Type` first or use `Match`.

## Expose Attribute

`[Expose(typeof(IFoo), typeof(IBar))]` generates interface implementations on the union by delegating each member call to the currently active variant via a `switch` on `_type`.

**Requirement:** Every variant type listed in `[Union]` must implement every interface listed in `[Expose]`. The analyzer enforces this at compile time (see [UNION007](#diagnostics)).

```csharp
public interface IPuzzleCell
{
    short Id { get; }
}

[Union(typeof(VoidCell), typeof(EmptyCell), typeof(RegularCell))]
[Expose(typeof(IPuzzleCell))]
public partial struct PuzzleCell { }

// Now PuzzleCell implements IPuzzleCell:
PuzzleCell cell = new RegularCell(42);
IPuzzleCell iface = cell;
Console.WriteLine(iface.Id); // 42
```

### Interface Member Support Matrix

The `[Expose]` attribute generates delegation code for interface members. Not all C# interface member types are supported yet:

| Interface Member | Supported | Notes |
|---|:---:|---|
| **Properties** (get) | :white_check_mark: | Read-only, computed, auto-properties |
| **Properties** (set) | :white_check_mark: | Write-only and read-write |
| **Properties** (get + set) | :white_check_mark: | Both accessors delegated |
| **Methods** | :white_check_mark: | Void and non-void return types |
| **Methods** with `ref`/`out`/`in` params | :white_check_mark: | All parameter ref kinds supported |
| **Generic methods** | :white_check_mark: | Type parameters and constraints preserved |
| **Indexers** (get) | :white_check_mark: | Single and multi-parameter |
| **Indexers** (set) | :white_check_mark: | Single and multi-parameter |
| **Indexers** (get + set) | :white_check_mark: | Both accessors delegated |
| **Events** | :x: | Not yet implemented |
| **Static abstract members** | :x: | C# 11+ feature, not yet implemented |
| **Default interface methods** | :x: | Not delegated (inherited from interface directly) |

### C# Version Compatibility

The generator targets **netstandard2.0** and emits C# code compatible with older compilers. However, the variant types you write may use modern C# features. Here is what works where:

| Feature | Modern C# (.NET 6+) | Unity 2021.3+ (C# 9) | Notes |
|---|:---:|:---:|---|
| `[Union]` on `partial struct` | :white_check_mark: | :white_check_mark: | Core feature |
| `[Union]` on `partial class` | :white_check_mark: | :white_check_mark: | Core feature |
| `struct` variant types | :white_check_mark: | :white_check_mark: | Core feature |
| `class` variant types | :white_check_mark: | :white_check_mark: | Core feature |
| Nested type declarations | :white_check_mark: | :white_check_mark: | Arbitrary nesting depth |
| `[Expose]` interface delegation | :white_check_mark: | :white_check_mark: | Core feature |
| `[StructLayout(Explicit)]` overlap | :white_check_mark: | :white_check_mark: | Struct unions only |
| `Match(Action<A>, ...)` | :white_check_mark: | :white_check_mark: | Core feature |
| Implicit conversions | :white_check_mark: | :white_check_mark: | Core feature |
| Equality operators | :white_check_mark: | :white_check_mark: | Core feature |
| `readonly struct` variants | :white_check_mark: | :white_check_mark: | C# 7.2+ |
| Generic method constraints in `[Expose]` | :white_check_mark: | :white_check_mark: | `struct`, `class`, `new()`, `unmanaged`, `notnull` |
| `record struct` variants | :white_check_mark: | :x: | C# 10+ only |
| Primary constructors on variants | :white_check_mark: | :x: | C# 12+ only |
| Generic variant types | :white_check_mark: | :white_check_mark: | e.g. `Wrapper<int>` becomes `WrapperOfInt32` |
| `Match<TResult>(Func<A,T>, ...)` | :x: | :x: | Not yet implemented (void `Match` only) |

> **Note:** The *generated* code itself uses only C# features available in Unity 2021.3. The table above refers to what you can use in your *variant struct definitions*.

## Diagnostics

The package includes Roslyn analyzers that catch common mistakes at compile time:

| Code | Severity | Description |
|------|----------|-------------|
| **UNION002** | Error | Type with `[Union]` must be `partial` |
| **UNION003** | Error | Parent type of a nested `[Union]` type must be `partial` |
| **UNION004** | Error | `[Union]` must specify at least one type |
| **UNION005** | Error | Duplicate interface in `[Expose]` |
| **UNION006** | Error | `[Expose]` argument must be an interface, not a class/struct |
| **UNION007** | Error | A variant type does not implement an exposed interface |
| **UNION008** | Error | `[Expose]` requires `[Union]` on the same type |
| **UNION009** | Error | `[Expose]` must specify at least one interface |
| **UNION010** | Error | Duplicate type in `[Union]` |

## Limitations

**Known limitations in the current version (1.0.2):**

- **No `Match<TResult>`** -- the `Match` method only supports `void` callbacks (`Action<T>`). There is no overload that returns a value (`Func<T, TResult>`).
- **No events in `[Expose]`** -- interface events are silently skipped during code generation.
- **No static abstract members in `[Expose]`** -- C# 11+ static abstract/virtual interface members are not delegated.
- **Default value ambiguity** -- `default(Shape)` produces a union where `Type` is the first variant kind (value `0`) but all fields are zero-initialized. This may represent an unintended state.
- **Implicit conversion can throw** -- converting from union to variant (`Circle c = shape`) throws if the union holds a different variant. This is by design but can be surprising.
- **No `TryGet` pattern** -- there is no safe extraction method like `bool TryGetCircle(out Circle c)`. Use `Type` check or `Match` instead.

## License

[MIT](LICENSE) -- Appegy

