using NUnit.Framework;
using Appegy.Union.Cells;
using Appegy.Union.Cells.Variants;

namespace Appegy.Union
{
    [TestFixture]
    public class PuzzleCellToStringAndGetHashCodeTests
    {
        [Test]
        public void WhenCallingToString_AndTypeIsVoidCell_ThenReturnsVoidCellToString()
        {
            // Arrange
            var voidCell = new VoidCell();
            var puzzleCell = new PuzzleCell(voidCell);

            // Act
            var result = puzzleCell.ToString();

            // Assert
            Assert.AreEqual(voidCell.ToString(), result);
        }

        [Test]
        public void WhenCallingToString_AndTypeIsEmptyCell_ThenReturnsEmptyCellToString()
        {
            // Arrange
            var emptyCell = new EmptyCell();
            var puzzleCell = new PuzzleCell(emptyCell);

            // Act
            var result = puzzleCell.ToString();

            // Assert
            Assert.AreEqual(emptyCell.ToString(), result);
        }

        [Test]
        public void WhenCallingToString_AndTypeIsRegularCell_ThenReturnsRegularCellToString()
        {
            // Arrange
            var regularCell = new RegularCell(1); // Unique ID
            var puzzleCell = new PuzzleCell(regularCell);

            // Act
            var result = puzzleCell.ToString();

            // Assert
            Assert.AreEqual(regularCell.ToString(), result);
        }

        [Test]
        public void WhenCallingGetHashCode_AndTypeIsVoidCell_ThenReturnsVoidCellGetHashCode()
        {
            // Arrange
            var voidCell = new VoidCell();
            var puzzleCell = new PuzzleCell(voidCell);

            // Act
            var result = puzzleCell.GetHashCode();

            // Assert
            Assert.AreEqual(voidCell.GetHashCode(), result);
        }

        [Test]
        public void WhenCallingGetHashCode_AndTypeIsEmptyCell_ThenReturnsEmptyCellGetHashCode()
        {
            // Arrange
            var emptyCell = new EmptyCell();
            var puzzleCell = new PuzzleCell(emptyCell);

            // Act
            var result = puzzleCell.GetHashCode();

            // Assert
            Assert.AreEqual(emptyCell.GetHashCode(), result);
        }

        [Test]
        public void WhenCallingGetHashCode_AndTypeIsRegularCell_ThenReturnsRegularCellGetHashCode()
        {
            // Arrange
            var regularCell = new RegularCell(1); // Unique ID
            var puzzleCell = new PuzzleCell(regularCell);

            // Act
            var result = puzzleCell.GetHashCode();

            // Assert
            Assert.AreEqual(regularCell.GetHashCode(), result);
        }
    }
}