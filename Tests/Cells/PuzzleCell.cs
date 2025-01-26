using System;
using Appegy.Union.Cells.Variants;

namespace Appegy.Union.Cells
{
    [Union(typeof(VoidCell), typeof(EmptyCell), typeof(RegularCell))]
    [Expose(typeof(IPuzzleCell), typeof(IMatchableCell), typeof(IMovableCell))]
    public partial struct PuzzleCell
    {
    }
}