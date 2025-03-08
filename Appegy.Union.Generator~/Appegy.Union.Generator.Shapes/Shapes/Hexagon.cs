namespace Appegy.Union.Generator.Shapes;

public readonly struct Hexagon(double sideLength) : IShape, IEquatable<Hexagon>
{
    public double SideLength { get; } = sideLength;

    public double Area => 3 * Math.Sqrt(3) * SideLength * SideLength / 2;

    public override string ToString()
    {
        return nameof(Hexagon);
    }

    public bool Equals(Hexagon other)
    {
        return SideLength.Equals(other.SideLength);
    }

    public override bool Equals(object? boxed)
    {
        return boxed is Hexagon other && Equals(other);
    }

    public override int GetHashCode()
    {
        return SideLength.GetHashCode();
    }

    public static bool operator ==(Hexagon left, Hexagon right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Hexagon left, Hexagon right)
    {
        return !(left == right);
    }
}