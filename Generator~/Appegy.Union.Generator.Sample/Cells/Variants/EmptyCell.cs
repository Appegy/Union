using Appegy.Union.Generator.Sample.Cells.Interfaces;

namespace Appegy.Union.Generator.Sample.Cells.Variants
{
    public struct EmptyCell : IPuzzleCell, IMovableCell, IMatchableCell
    {
        public short Id => -1;
        public bool Movable => false;
        public bool Matchable => false;
    }
}