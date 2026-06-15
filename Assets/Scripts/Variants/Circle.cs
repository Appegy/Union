using System;

namespace Appegy.Union.Sample.Variants
{
    public readonly struct Circle : IShape, IEquatable<Circle>
    {
        public double Radius { get; }

        public double Area => Math.PI * Radius * Radius;

        public Circle(double radius)
        {
            Radius = radius;
        }

        public override string ToString()
        {
            return nameof(Circle);
        }

        public bool Equals(Circle other)
        {
            return Radius.Equals(other.Radius);
        }

        public override bool Equals(object boxed)
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
}