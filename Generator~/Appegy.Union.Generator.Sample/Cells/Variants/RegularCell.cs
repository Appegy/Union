using Appegy.Union.Generator.Sample.Cells.Interfaces;

namespace Appegy.Union.Generator.Sample.Cells.Variants
{
    public struct RegularCell : IPuzzleCell, IMovableCell, IMatchableCell
    {
        public short Id { get; set; }
        public bool Movable => true;
        public bool Matchable => true;

        public RegularCell(short id)
        {
            Id = id;
        }
    }
}