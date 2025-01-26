using System;

namespace Appegy.Union.Cells.Variants
{
    public struct RegularCell : IPuzzleCell, IMovableCell, IMatchableCell, IEquatable<RegularCell>
    {
        public short Id { get; }
        public bool Movable => true;
        public bool Matchable => true;

        public RegularCell(short id)
        {
            Id = id;
        }

        public bool Equals(RegularCell other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object boxed)
        {
            return boxed is RegularCell other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}