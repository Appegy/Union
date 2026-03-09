# Testing Patterns

**Analysis Date:** 2026-03-09

## Test Framework

**Runner:**
- NUnit 3.13.3
- Config: `Appegy.Union.Generator~/Appegy.Union.Generator.Tests/Appegy.Union.Generator.Tests.csproj`
- Test adapter: NUnit3TestAdapter 4.5.0
- Test SDK: Microsoft.NET.Test.Sdk 17.13.0
- Target framework: net9.0

**Assertion Library:**
- NUnit built-in assertions (`Assert.AreEqual`, `Assert.IsTrue`, `Assert.IsFalse`, `Assert.Throws`)

**Run Commands:**
```bash
dotnet test Appegy.Union.Generator~/Appegy.Union.Generator.Tests/Appegy.Union.Generator.Tests.csproj    # Run all tests
dotnet test Appegy.Union.Generator~/Appegy.Union.Generator.Tests/Appegy.Union.Generator.Tests.csproj --configuration Release --verbosity normal   # CI mode
```

## Test File Organization

**Location:**
- Separate test project: `Appegy.Union.Generator~/Appegy.Union.Generator.Tests/`
- Tests mirror the Shapes project structure under `Shapes/` subdirectory

**Naming:**
- Test files: `{Subject}{Aspect}Tests.cs`
- Examples: `ShapeComparisonTests.cs`, `ShapeConstructorTests.cs`, `ShapePropertyTests.cs`

**Structure:**
```
Appegy.Union.Generator~/
  Appegy.Union.Generator.Tests/
    Shapes/
      ShapeComparisonTests.cs
      ShapeConstructorTests.cs
      ShapeInequalityOperatorTests.cs
      ShapeInterfaceAreaTests.cs
      ShapePropertyTests.cs
      ShapeToStringAndGetHashCodeTests.cs
      ShapeTypeChangeTests.cs
```

## Test Structure

**Suite Organization:**
```csharp
using Appegy.Union.Generator.Shapes;
using NUnit.Framework;

namespace Appegy.Union.Generator.Tests.Shapes;

[TestFixture]
public class ShapeComparisonTests
{
    [Test]
    public void WhenComparingShapeWithShape_AndValuesAreEqualCircle_ThenReturnsTrue()
    {
        // Arrange
        var circle = new Circle(5);
        var shape1 = new Shape(circle);
        var shape2 = new Shape(circle);

        // Act
        var equalsResult = shape1.Equals(shape2);
        var operatorResult = shape1 == shape2;

        // Assert
        Assert.IsTrue(equalsResult);
        Assert.IsTrue(operatorResult);
    }
}
```

**Patterns:**
- Every test class uses `[TestFixture]` attribute
- Every test method uses `[Test]` attribute
- Strict Arrange/Act/Assert pattern with `// Arrange`, `// Act`, `// Assert` comments in every test
- Combined `// Act & Assert` comment when testing exceptions
- Test method naming: `When{Action}_And{Condition}_Then{ExpectedResult}()` (BDD-style)
- No `[SetUp]` or `[TearDown]` methods; each test is self-contained
- No parameterized tests (`[TestCase]`); each variant is a separate method

## Test Approach

**What is Tested:**
- Tests validate the SOURCE-GENERATED code, not the generator itself
- The test project references the generator as an analyzer (`OutputItemType="Analyzer" ReferenceOutputAssembly="false"`)
- The `Appegy.Union.Generator.Shapes` project defines test shapes (`Shape`, `Circle`, `Rectangle`, `Hexagon`) that use the `[Union]` and `[Expose]` attributes
- Tests exercise the generated partial struct members at runtime

**Test Categories by File:**
- `ShapeConstructorTests.cs`: Verify union construction sets correct `Kind` type
- `ShapePropertyTests.cs`: Verify variant property getters/setters and exception on wrong type access
- `ShapeComparisonTests.cs`: Verify `Equals()` and `==` operator for same/different types
- `ShapeInequalityOperatorTests.cs`: Verify `!=` operator for same/different values
- `ShapeToStringAndGetHashCodeTests.cs`: Verify `ToString()` and `GetHashCode()` delegate to inner type
- `ShapeTypeChangeTests.cs`: Verify setting a different variant property updates `Kind`
- `ShapeInterfaceAreaTests.cs`: Verify `[Expose]` interface property delegation works

## Mocking

**Framework:** None

**What to Mock:** Nothing is mocked. All tests are integration-style, exercising real generated code against real struct types.

**What NOT to Mock:** Everything -- the test strategy relies on compiling generated source and running it directly.

## Fixtures and Factories

**Test Data:**
```csharp
// Inline construction in each test -- no shared factories
var circle = new Circle(5);
var rectangle = new Rectangle(4, 6);
var hexagon = new Hexagon(3);
```

**Location:**
- Test shapes are defined in a separate project: `Appegy.Union.Generator~/Appegy.Union.Generator.Shapes/`
- Key files:
  - `Appegy.Union.Generator~/Appegy.Union.Generator.Shapes/Shapes/Shape.cs` - Union definition
  - `Appegy.Union.Generator~/Appegy.Union.Generator.Shapes/Shapes/Variants/Circle.cs`
  - `Appegy.Union.Generator~/Appegy.Union.Generator.Shapes/Shapes/Variants/Rectangle.cs`
  - `Appegy.Union.Generator~/Appegy.Union.Generator.Shapes/Shapes/Variants/Hexagon.cs`
  - `Appegy.Union.Generator~/Appegy.Union.Generator.Shapes/Shapes/Attributes/UnionAttribute.cs` - Local copy of attribute
  - `Appegy.Union.Generator~/Appegy.Union.Generator.Shapes/Shapes/Attributes/ExposeAttribute.cs` - Local copy of attribute

## Coverage

**Requirements:** None enforced. No coverage tool configured.

**View Coverage:** Not configured. Add `coverlet.collector` package to enable:
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Test Types

**Unit Tests:**
- Not present. There are no tests for individual generator parts, analyzers, or utility methods in isolation.

**Integration Tests:**
- All tests are integration tests that validate the end-to-end generated code output
- The source generator runs at compile time on the Shapes project, and tests verify the runtime behavior of generated members

**E2E Tests:**
- Not used

**Diagnostic/Analyzer Tests:**
- Not present. No tests verify that analyzer diagnostics (UNION002-UNION010) are correctly reported.

## Common Patterns

**Equality Testing:**
```csharp
[Test]
public void WhenComparingShapeWithShape_AndValuesAreEqualCircle_ThenReturnsTrue()
{
    // Arrange
    var circle = new Circle(5);
    var shape1 = new Shape(circle);
    var shape2 = new Shape(circle);

    // Act
    var equalsResult = shape1.Equals(shape2);
    var operatorResult = shape1 == shape2;

    // Assert
    Assert.IsTrue(equalsResult);
    Assert.IsTrue(operatorResult);
}
```

**Exception Testing:**
```csharp
[Test]
public void WhenGettingCircle_AndTypeIsNotCircle_ThenThrowsException()
{
    // Arrange
    var rectangle = new Rectangle(4, 6);
    var shape = new Shape(rectangle);

    // Act & Assert
    Assert.Throws<Exception>(() => { _ = shape.Circle; });
}
```

**Interface Delegation Testing:**
```csharp
[Test]
public void WhenAccessingAreaViaInterface_AndTypeIsCircle_ThenReturnsCircleArea()
{
    // Arrange
    var circle = new Circle(5);
    var union = new Shape(circle);
    IShape shape = union;

    // Act
    var area = shape.Area;

    // Assert
    Assert.AreEqual(circle.Area, area);
}
```

**Type Mutation Testing:**
```csharp
[Test]
public void WhenChangingShapeFromCircleToRectangle_ThenTypeUpdatesCorrectly()
{
    // Arrange
    var shape = new Shape(new Circle(5));

    // Act
    shape.Rectangle = new Rectangle(4, 6);

    // Assert
    Assert.AreEqual(Shape.Kind.Rectangle, shape.Type);
    Assert.AreEqual(new Rectangle(4, 6), shape.Rectangle);
}
```

## CI Integration

**GitHub Actions:** `.github/workflows/run-tests.yml`
- Triggers on: pull_request, workflow_dispatch
- Runner: ubuntu-latest
- .NET SDK: 9.0.x
- Steps: restore -> build (Release) -> test (Release)
- Solution file: `Appegy.Union.Generator~/Appegy.Union.Generator.slnx`

## Adding New Tests

**New test for existing union behavior:**
1. Add test file to `Appegy.Union.Generator~/Appegy.Union.Generator.Tests/Shapes/`
2. Name it `Shape{Aspect}Tests.cs`
3. Use `namespace Appegy.Union.Generator.Tests.Shapes;`
4. Add `[TestFixture]` class with `[Test]` methods
5. Follow `When{Action}_And{Condition}_Then{Result}` naming
6. Use `// Arrange` / `// Act` / `// Assert` comments

**New test shapes:**
1. Add variant struct to `Appegy.Union.Generator~/Appegy.Union.Generator.Shapes/Shapes/Variants/`
2. Implement `IShape`, `IEquatable<T>`, override `ToString()`, `Equals()`, `GetHashCode()`, and operators
3. Add the type to the `[Union(...)]` attribute on `Shape` in `Appegy.Union.Generator~/Appegy.Union.Generator.Shapes/Shapes/Shape.cs`

---

*Testing analysis: 2026-03-09*
