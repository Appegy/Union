namespace Appegy.Union.Generator.Tests.Results;

[Union(typeof(VoidCell), typeof(EmptyCell), typeof(RegularCell))]
[Expose(typeof(IPuzzleCell), typeof(IMatchableCell), typeof(IMovableCell))]
public partial struct PuzzleCell;