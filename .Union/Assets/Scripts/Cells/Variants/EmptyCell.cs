﻿namespace Appegy.Union.Sample.Cells
{
    public struct EmptyCell : IPuzzleCell, IMovableCell, IMatchableCell
    {
        public short Id => -1;
        public bool Movable => false;
        public bool Matchable => false;
    }
}