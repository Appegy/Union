using System;
using Appegy.Union.Generator.Shapes;
using NUnit.Framework;

namespace Appegy.Union.Generator.Tests.Shapes;

[TestFixture]
public class ShapePropertyTests
{
    [Test]
    public void WhenGettingCircle_AndTypeIsCircle_ThenReturnsCircle()
    {
        // Arrange
        var circle = new Circle(5);
        var shape = new Shape(circle);

        // Act
        var result = shape.Circle;

        // Assert
        Assert.AreEqual(circle, result);
    }

    [Test]
    public void WhenGettingCircle_AndTypeIsNotCircle_ThenThrowsException()
    {
        // Arrange
        var rectangle = new Rectangle(4, 6);
        var shape = new Shape(rectangle);

        // Act & Assert
        Assert.Throws<Exception>(() => { _ = shape.Circle; });
    }

    [Test]
    public void WhenSettingCircle_ThenTypeIsCircle()
    {
        // Arrange
        var shape = new Shape(new Hexagon());
        var circle = new Circle(5);

        // Act
        shape.Circle = circle;

        // Assert
        Assert.AreEqual(Shape.Kind.Circle, shape.Type);
        Assert.AreEqual(circle, shape.Circle);
    }

    [Test]
    public void WhenGettingRectangle_AndTypeIsRectangle_ThenReturnsRectangle()
    {
        // Arrange
        var rectangle = new Rectangle(4, 6);
        var shape = new Shape(rectangle);

        // Act
        var result = shape.Rectangle;

        // Assert
        Assert.AreEqual(rectangle, result);
    }

    [Test]
    public void WhenGettingRectangle_AndTypeIsNotRectangle_ThenThrowsException()
    {
        // Arrange
        var hexagon = new Hexagon(3);
        var shape = new Shape(hexagon);

        // Act & Assert
        Assert.Throws<Exception>(() => { _ = shape.Rectangle; });
    }

    [Test]
    public void WhenSettingRectangle_ThenTypeIsRectangle()
    {
        // Arrange
        var shape = new Shape(new Hexagon());
        var rectangle = new Rectangle(4, 6);

        // Act
        shape.Rectangle = rectangle;

        // Assert
        Assert.AreEqual(Shape.Kind.Rectangle, shape.Type);
        Assert.AreEqual(rectangle, shape.Rectangle);
    }

    [Test]
    public void WhenGettingHexagon_AndTypeIsHexagon_ThenReturnsHexagon()
    {
        // Arrange
        var hexagon = new Hexagon(3);
        var shape = new Shape(hexagon);

        // Act
        var result = shape.Hexagon;

        // Assert
        Assert.AreEqual(hexagon, result);
    }

    [Test]
    public void WhenGettingHexagon_AndTypeIsNotHexagon_ThenThrowsException()
    {
        // Arrange
        var circle = new Circle(5);
        var shape = new Shape(circle);

        // Act & Assert
        Assert.Throws<Exception>(() => { _ = shape.Hexagon; });
    }

    [Test]
    public void WhenSettingHexagon_ThenTypeIsHexagon()
    {
        // Arrange
        var shape = new Shape(new Circle());
        var hexagon = new Hexagon(3);

        // Act
        shape.Hexagon = hexagon;

        // Assert
        Assert.AreEqual(Shape.Kind.Hexagon, shape.Type);
        Assert.AreEqual(hexagon, shape.Hexagon);
    }
}
