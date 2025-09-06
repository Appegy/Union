namespace Appegy.Union.Generator.Shapes;

public interface IShape
{
    double Area { get; }
}

[Union(typeof(Circle), typeof(Rectangle), typeof(Hexagon))]
[Expose(typeof(IShape))]
public partial struct Shape;