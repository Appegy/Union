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

    [Test]
    public void WhenComparingShapeWithShape_AndValuesAreEqualRectangle_ThenReturnsTrue()
    {
        // Arrange
        var rectangle = new Rectangle(4, 6);
        var shape1 = new Shape(rectangle);
        var shape2 = new Shape(rectangle);

        // Act
        var equalsResult = shape1.Equals(shape2);
        var operatorResult = shape1 == shape2;

        // Assert
        Assert.IsTrue(equalsResult);
        Assert.IsTrue(operatorResult);
    }

    [Test]
    public void WhenComparingShapeWithShape_AndValuesAreEqualHexagon_ThenReturnsTrue()
    {
        // Arrange
        var hexagon = new Hexagon(3);
        var shape1 = new Shape(hexagon);
        var shape2 = new Shape(hexagon);

        // Act
        var equalsResult = shape1.Equals(shape2);
        var operatorResult = shape1 == shape2;

        // Assert
        Assert.IsTrue(equalsResult);
        Assert.IsTrue(operatorResult);
    }

    [Test]
    public void WhenComparingShapeWithShape_AndValuesAreNotEqual_ThenReturnsFalse()
    {
        // Arrange
        var circle = new Circle(5);
        var rectangle = new Rectangle(4, 6);
        var shape1 = new Shape(circle);
        var shape2 = new Shape(rectangle);

        // Act
        var equalsResult = shape1.Equals(shape2);
        var operatorResult = shape1 == shape2;

        // Assert
        Assert.IsFalse(equalsResult);
        Assert.IsFalse(operatorResult);
    }

    [Test]
    public void WhenComparingShapeWithCircle_AndValuesAreEqual_ThenReturnsTrue()
    {
        // Arrange
        var circle = new Circle(5);
        var shape = new Shape(circle);

        // Act
        var equalsResult = shape.Equals(circle);
        var operatorResult = shape == circle;

        // Assert
        Assert.IsTrue(equalsResult);
        Assert.IsTrue(operatorResult);
    }

    [Test]
    public void WhenComparingShapeWithRectangle_AndValuesAreEqual_ThenReturnsTrue()
    {
        // Arrange
        var rectangle = new Rectangle(4, 6);
        var shape = new Shape(rectangle);

        // Act
        var equalsResult = shape.Equals(rectangle);
        var operatorResult = shape == rectangle;

        // Assert
        Assert.IsTrue(equalsResult);
        Assert.IsTrue(operatorResult);
    }

    [Test]
    public void WhenComparingShapeWithHexagon_AndValuesAreEqual_ThenReturnsTrue()
    {
        // Arrange
        var hexagon = new Hexagon(3);
        var shape = new Shape(hexagon);

        // Act
        var equalsResult = shape.Equals(hexagon);
        var operatorResult = shape == hexagon;

        // Assert
        Assert.IsTrue(equalsResult);
        Assert.IsTrue(operatorResult);
    }
}
