using System;
using Appegy.Union.Generator.Sample.Cells;
using Appegy.Union.Generator.Sample.Cells.Variants;

namespace Appegy.Union.Generator.Sample;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello World!");
        var voidCell = new PuzzleCell(new VoidCell());
        var emptyCell = new PuzzleCell(new EmptyCell());
        var regularCell = new PuzzleCell(new RegularCell(1));

        Console.WriteLine(voidCell.Type);
        Console.WriteLine(emptyCell.Type);
        Console.WriteLine(regularCell.Type);
    }
}