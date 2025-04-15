using System;

namespace Appegy.Union.Sample.Variants
{
    public readonly struct Rectangle : IShape, IEquatable<Rectangle>
    {
        public double Width { get; }
        public double Height { get; }
        public double Area => Width * Height;

        public Rectangle(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public override string ToString()
        {
            return nameof(Rectangle);
        }

        public bool Equals(Rectangle other)
        {
            return Width.Equals(other.Width) && Height.Equals(other.Height);
        }

        public override bool Equals(object boxed)
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
}