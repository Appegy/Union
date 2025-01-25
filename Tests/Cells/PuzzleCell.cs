namespace Appegy.Union.Cells
{
    [Expose(typeof(IPuzzleCell), typeof(IMatchableCell), typeof(IMovableCell))]
    [Union(typeof(VoidCell), typeof(EmptyCell), typeof(RegularCell))]
    public partial struct PuzzleCell
    {
    }
}