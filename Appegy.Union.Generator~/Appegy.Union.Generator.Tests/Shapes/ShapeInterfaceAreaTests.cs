using Appegy.Union.Generator.Shapes;
using NUnit.Framework;

namespace Appegy.Union.Generator.Tests.Shapes;

[TestFixture]
public class ShapeInterfaceAreaTests
{
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

    [Test]
    public void WhenAccessingAreaViaInterface_AndTypeIsRectangle_ThenReturnsRectangleArea()
    {
        // Arrange
        var rectangle = new Rectangle(4, 6);
        var union = new Shape(rectangle);
        IShape shape = union;

        // Act
        var area = shape.Area;

        // Assert
        Assert.AreEqual(rectangle.Area, area);
    }

    [Test]
    public void WhenAccessingAreaViaInterface_AndTypeIsHexagon_ThenReturnsHexagonArea()
    {
        // Arrange
        var hexagon = new Hexagon(3);
        var union = new Shape(hexagon);
        IShape shape = union;

        // Act
        var area = shape.Area;

        // Assert
        Assert.AreEqual(hexagon.Area, area);
    }
}
