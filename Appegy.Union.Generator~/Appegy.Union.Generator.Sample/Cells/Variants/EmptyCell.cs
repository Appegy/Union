using System;

namespace Appegy.Union.Generator.Sample.Cells
{
    public struct EmptyCell : IPuzzleCell, IMovableCell, IMatchableCell, IEquatable<EmptyCell>
    {
        public short Id => -1;
        public bool Movable => false;
        public bool Matchable => false;

        public bool Equals(EmptyCell other)
        {
            return true;
        }

        public override bool Equals(object? boxed)
        {
            return boxed is EmptyCell other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}