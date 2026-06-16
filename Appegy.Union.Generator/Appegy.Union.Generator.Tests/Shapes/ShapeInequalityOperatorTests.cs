using Appegy.Union.Generator.Shapes;
using NUnit.Framework;

namespace Appegy.Union.Generator.Tests.Shapes;

[TestFixture]
public class ShapeInequalityOperatorTests
{
    [Test]
    public void WhenComparingShapeWithShape_AndValuesAreEqual_ThenNotEqualsReturnsFalse()
    {
        // Arrange
        var circle = new Circle(5);
        var shape1 = new Shape(circle);
        var shape2 = new Shape(circle);

        // Act
        var operatorResult = shape1 != shape2;

        // Assert
        Assert.IsFalse(operatorResult);
    }

    [Test]
    public void WhenComparingShapeWithShape_AndValuesAreNotEqual_ThenNotEqualsReturnsTrue()
    {
        // Arrange
        var circle = new Circle(5);
        var rectangle = new Rectangle(4, 6);
        var shape1 = new Shape(circle);
        var shape2 = new Shape(rectangle);

        // Act
        var operatorResult = shape1 != shape2;

        // Assert
        Assert.IsTrue(operatorResult);
    }

    [Test]
    public void WhenComparingShapeWithCircle_AndValuesAreEqual_ThenNotEqualsReturnsFalse()
    {
        // Arrange
        var circle = new Circle(5);
        var shape = new Shape(circle);

        // Act
        var operatorResult = shape != circle;

        // Assert
        Assert.IsFalse(operatorResult);
    }

    [Test]
    public void WhenComparingShapeWithCircle_AndValuesAreNotEqual_ThenNotEqualsReturnsTrue()
    {
        // Arrange
        var circle1 = new Circle(5);
        var circle2 = new Circle(7);
        var shape = new Shape(circle1);

        // Act
        var operatorResult = shape != circle2;

        // Assert
        Assert.IsTrue(operatorResult);
    }

    [Test]
    public void WhenComparingShapeWithRectangle_AndValuesAreEqual_ThenNotEqualsReturnsFalse()
    {
        // Arrange
        var rectangle = new Rectangle(4, 6);
        var shape = new Shape(rectangle);

        // Act
        var operatorResult = shape != rectangle;

        // Assert
        Assert.IsFalse(operatorResult);
    }

    [Test]
    public void WhenComparingShapeWithRectangle_AndValuesAreNotEqual_ThenNotEqualsReturnsTrue()
    {
        // Arrange
        var rectangle1 = new Rectangle(4, 6);
        var rectangle2 = new Rectangle(3, 8);
        var shape = new Shape(rectangle1);

        // Act
        var operatorResult = shape != rectangle2;

        // Assert
        Assert.IsTrue(operatorResult);
    }

    [Test]
    public void WhenComparingShapeWithHexagon_AndValuesAreEqual_ThenNotEqualsReturnsFalse()
    {
        // Arrange
        var hexagon = new Hexagon(3);
        var shape = new Shape(hexagon);

        // Act
        var operatorResult = shape != hexagon;

        // Assert
        Assert.IsFalse(operatorResult);
    }

    [Test]
    public void WhenComparingShapeWithHexagon_AndValuesAreNotEqual_ThenNotEqualsReturnsTrue()
    {
        // Arrange
        var hexagon1 = new Hexagon(3);
        var hexagon2 = new Hexagon(4);
        var shape = new Shape(hexagon1);

        // Act
        var operatorResult = shape != hexagon2;

        // Assert
        Assert.IsTrue(operatorResult);
    }
}
