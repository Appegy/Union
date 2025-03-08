using Appegy.Union.Generator.Shapes;
using NUnit.Framework;

namespace Appegy.Union.Generator.Tests.Shapes;

[TestFixture]
public class ShapeToStringAndGetHashCodeTests
{
    [Test]
    public void WhenCallingToString_AndTypeIsCircle_ThenReturnsCircleToString()
    {
        // Arrange
        var circle = new Circle(5);
        var shape = new Shape(circle);

        // Act
        var result = shape.ToString();

        // Assert
        Assert.AreEqual(circle.ToString(), result);
    }

    [Test]
    public void WhenCallingToString_AndTypeIsRectangle_ThenReturnsRectangleToString()
    {
        // Arrange
        var rectangle = new Rectangle(4, 6);
        var shape = new Shape(rectangle);

        // Act
        var result = shape.ToString();

        // Assert
        Assert.AreEqual(rectangle.ToString(), result);
    }

    [Test]
    public void WhenCallingToString_AndTypeIsHexagon_ThenReturnsHexagonToString()
    {
        // Arrange
        var hexagon = new Hexagon(3);
        var shape = new Shape(hexagon);

        // Act
        var result = shape.ToString();

        // Assert
        Assert.AreEqual(hexagon.ToString(), result);
    }

    [Test]
    public void WhenCallingGetHashCode_AndTypeIsCircle_ThenReturnsCircleGetHashCode()
    {
        // Arrange
        var circle = new Circle(5);
        var shape = new Shape(circle);

        // Act
        var result = shape.GetHashCode();

        // Assert
        Assert.AreEqual(circle.GetHashCode(), result);
    }

    [Test]
    public void WhenCallingGetHashCode_AndTypeIsRectangle_ThenReturnsRectangleGetHashCode()
    {
        // Arrange
        var rectangle = new Rectangle(4, 6);
        var shape = new Shape(rectangle);

        // Act
        var result = shape.GetHashCode();

        // Assert
        Assert.AreEqual(rectangle.GetHashCode(), result);
    }

    [Test]
    public void WhenCallingGetHashCode_AndTypeIsHexagon_ThenReturnsHexagonGetHashCode()
    {
        // Arrange
        var hexagon = new Hexagon(3);
        var shape = new Shape(hexagon);

        // Act
        var result = shape.GetHashCode();

        // Assert
        Assert.AreEqual(hexagon.GetHashCode(), result);
    }
}
