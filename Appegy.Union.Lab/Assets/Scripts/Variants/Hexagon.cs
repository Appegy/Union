using System;

namespace Appegy.Union.Sample.Variants
{
    public readonly struct Hexagon : IShape, IEquatable<Hexagon>
    {
        public double SideLength { get; }

        public double Area => 3 * Math.Sqrt(3) * SideLength * SideLength / 2;

        public Hexagon(double sideLength)
        {
            SideLength = sideLength;
        }

        public override string ToString()
        {
            return nameof(Hexagon);
        }

        public bool Equals(Hexagon other)
        {
            return SideLength.Equals(other.SideLength);
        }

        public override bool Equals(object boxed)
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
}