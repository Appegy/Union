using Appegy.Union.Sample.Variants;

namespace Appegy.Union.Sample
{
    public interface IShape
    {
        double Area { get; }
    }

    [Union(typeof(Circle), typeof(Rectangle), typeof(Hexagon))]
    [Expose(typeof(IShape))]
    public partial struct Shape
    {
    }
}