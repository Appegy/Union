using Appegy.Union.Generator.Shapes;
using NUnit.Framework;

namespace Appegy.Union.Generator.Tests.Shapes;

[TestFixture]
public class ShapeTypeChangeTests
{
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

    [Test]
    public void WhenChangingShapeFromCircleToHexagon_ThenTypeUpdatesCorrectly()
    {
        // Arrange
        var shape = new Shape(new Circle(5));

        // Act
        shape.Hexagon = new Hexagon(3);

        // Assert
        Assert.AreEqual(Shape.Kind.Hexagon, shape.Type);
        Assert.AreEqual(new Hexagon(3), shape.Hexagon);
    }

    [Test]
    public void WhenChangingShapeFromRectangleToCircle_ThenTypeUpdatesCorrectly()
    {
        // Arrange
        var shape = new Shape(new Rectangle(4, 6));

        // Act
        shape.Circle = new Circle(5);

        // Assert
        Assert.AreEqual(Shape.Kind.Circle, shape.Type);
        Assert.AreEqual(new Circle(5), shape.Circle);
    }

    [Test]
    public void WhenChangingShapeFromRectangleToHexagon_ThenTypeUpdatesCorrectly()
    {
        // Arrange
        var shape = new Shape(new Rectangle(4, 6));

        // Act
        shape.Hexagon = new Hexagon(3);

        // Assert
        Assert.AreEqual(Shape.Kind.Hexagon, shape.Type);
        Assert.AreEqual(new Hexagon(3), shape.Hexagon);
    }

    [Test]
    public void WhenChangingShapeFromHexagonToCircle_ThenTypeUpdatesCorrectly()
    {
        // Arrange
        var shape = new Shape(new Hexagon(3));

        // Act
        shape.Circle = new Circle(5);

        // Assert
        Assert.AreEqual(Shape.Kind.Circle, shape.Type);
        Assert.AreEqual(new Circle(5), shape.Circle);
    }

    [Test]
    public void WhenChangingShapeFromHexagonToRectangle_ThenTypeUpdatesCorrectly()
    {
        // Arrange
        var shape = new Shape(new Hexagon(3));

        // Act
        shape.Rectangle = new Rectangle(4, 6);

        // Assert
        Assert.AreEqual(Shape.Kind.Rectangle, shape.Type);
        Assert.AreEqual(new Rectangle(4, 6), shape.Rectangle);
    }
}
