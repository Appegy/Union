using System;

namespace Appegy.Union.Generator.Shapes;

public readonly struct Circle(double radius) : IShape, IEquatable<Circle>
{
    public double Radius { get; } = radius;

    public double Area => Math.PI * Radius * Radius;

    public override string ToString()
    {
        return nameof(Circle);
    }

    public bool Equals(Circle other)
    {
        return Radius.Equals(other.Radius);
    }

    public override bool Equals(object? boxed)
    {
        return boxed is Circle other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Radius.GetHashCode();
    }

    public static bool operator ==(Circle left, Circle right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Circle left, Circle right)
    {
        return !(left == right);
    }
}