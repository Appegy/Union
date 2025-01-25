namespace Appegy.Union.Cells
{
    public struct VoidCell : IPuzzleCell, IMovableCell, IMatchableCell
    {
        public short Id => -2;
        public bool Movable => false;
        public bool Matchable => false;
    }
}