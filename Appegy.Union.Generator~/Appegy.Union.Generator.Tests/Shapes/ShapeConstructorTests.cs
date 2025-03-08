using Appegy.Union.Generator.Shapes;
using NUnit.Framework;

namespace Appegy.Union.Generator.Tests.Shapes;

[TestFixture]
public class ShapeConstructorTests
{
    [Test]
    public void WhenCreatingShapeWithCircle_ThenTypeIsCircle()
    {
        // Arrange
        var circle = new Circle(5);

        // Act
        var shape = new Shape(circle);

        // Assert
        Assert.AreEqual(Shape.Kind.Circle, shape.Type);
    }

    [Test]
    public void WhenCreatingShapeWithRectangle_ThenTypeIsRectangle()
    {
        // Arrange
        var rectangle = new Rectangle(4, 6);

        // Act
        var shape = new Shape(rectangle);

        // Assert
        Assert.AreEqual(Shape.Kind.Rectangle, shape.Type);
    }

    [Test]
    public void WhenCreatingShapeWithHexagon_ThenTypeIsHexagon()
    {
        // Arrange
        var hexagon = new Hexagon(3);

        // Act
        var shape = new Shape(hexagon);

        // Assert
        Assert.AreEqual(Shape.Kind.Hexagon, shape.Type);
    }
}