using System;

namespace Appegy.Union.Generator.Shapes;

public readonly struct Rectangle(double width, double height) : IShape, IEquatable<Rectangle>
{
    public double Width { get; } = width;
    public double Height { get; } = height;
    public double Area => Width * Height;

    public override string ToString()
    {
        return nameof(Rectangle);
    }

    public bool Equals(Rectangle other)
    {
        return Width.Equals(other.Width) && Height.Equals(other.Height);
    }

    public override bool Equals(object? boxed)
    {
        return boxed is Rectangle other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }

    public static bool operator ==(Rectangle left, Rectangle right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Rectangle left, Rectangle right)
    {
        return !(left == right);
    }
}