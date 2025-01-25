using Appegy.Union.Generator.Sample.Cells.Interfaces;
using Appegy.Union.Generator.Sample.Cells.Variants;

namespace Appegy.Union.Generator.Sample.Cells
{
    [Expose(typeof(IPuzzleCell), typeof(IMatchableCell), typeof(IMovableCell))]
    [Union(typeof(VoidCell), typeof(EmptyCell), typeof(RegularCell))]
    public partial struct PuzzleCell
    {
    }
}