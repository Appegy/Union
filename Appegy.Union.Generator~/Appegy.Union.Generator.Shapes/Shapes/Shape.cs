using System.Runtime.InteropServices;

namespace Appegy.Union.Generator.Shapes;

public interface IShape
{
    double Area { get; }
}

[Union(typeof(Circle), typeof(Rectangle), typeof(Hexagon))]
[Expose(typeof(IShape))]
[StructLayout(LayoutKind.Explicit, Pack = 1)]
public partial struct Shape;