using System;

namespace Appegy.Union.Generator.Tests.Results
{
    public struct VoidCell : IPuzzleCell, IMovableCell, IMatchableCell, IEquatable<VoidCell>
    {
        public short Id => -2;
        public bool Movable => false;
        public bool Matchable => false;

        public bool Equals(VoidCell other)
        {
            return true;
        }

        public override bool Equals(object? boxed)
        {
            return boxed is VoidCell other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}