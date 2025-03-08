using Appegy.Union.Cells;
using Appegy.Union.Cells.Variants;
using NUnit.Framework;

namespace Appegy.Union
{
    [TestFixture]
    public class PuzzleCellComparisonTests
    {
        [Test]
        public void WhenComparingPuzzleCellWithPuzzleCell_AndValuesAreEqualVoidCell_ThenReturnsTrue()
        {
            // Arrange
            var voidCell = new VoidCell();
            var puzzleCell1 = new PuzzleCell(voidCell);
            var puzzleCell2 = new PuzzleCell(voidCell);

            // Act
            var equalsResult = puzzleCell1.Equals(puzzleCell2);
            var operatorResult = puzzleCell1 == puzzleCell2;

            // Assert
            Assert.IsTrue(equalsResult);
            Assert.IsTrue(operatorResult);
        }

        [Test]
        public void WhenComparingPuzzleCellWithPuzzleCell_AndValuesAreEqualEmptyCell_ThenReturnsTrue()
        {
            // Arrange
            var emptyCell = new EmptyCell();
            var puzzleCell1 = new PuzzleCell(emptyCell);
            var puzzleCell2 = new PuzzleCell(emptyCell);

            // Act
            var equalsResult = puzzleCell1.Equals(puzzleCell2);
            var operatorResult = puzzleCell1 == puzzleCell2;

            // Assert
            Assert.IsTrue(equalsResult);
            Assert.IsTrue(operatorResult);
        }

        [Test]
        public void WhenComparingPuzzleCellWithPuzzleCell_AndValuesAreEqualRegularCell_ThenReturnsTrue()
        {
            // Arrange
            var regularCell = new RegularCell();
            var puzzleCell1 = new PuzzleCell(regularCell);
            var puzzleCell2 = new PuzzleCell(regularCell);

            // Act
            var equalsResult = puzzleCell1.Equals(puzzleCell2);
            var operatorResult = puzzleCell1 == puzzleCell2;

            // Assert
            Assert.IsTrue(equalsResult);
            Assert.IsTrue(operatorResult);
        }

        [Test]
        public void WhenComparingPuzzleCellWithPuzzleCell_AndValuesAreNotEqual_ThenReturnsFalse()
        {
            // Arrange
            var voidCell = new VoidCell();
            var emptyCell = new EmptyCell();
            var puzzleCell1 = new PuzzleCell(voidCell);
            var puzzleCell2 = new PuzzleCell(emptyCell);

            // Act
            var equalsResult = puzzleCell1.Equals(puzzleCell2);
            var operatorResult = puzzleCell1 == puzzleCell2;

            // Assert
            Assert.IsFalse(equalsResult);
            Assert.IsFalse(operatorResult);
        }

        [Test]
        public void WhenComparingPuzzleCellWithVoidCell_AndValuesAreEqual_ThenReturnsTrue()
        {
            // Arrange
            var voidCell = new VoidCell();
            var puzzleCell = new PuzzleCell(voidCell);

            // Act
            var equalsResult = puzzleCell.Equals(voidCell);
            var operatorResult = puzzleCell == voidCell;

            // Assert
            Assert.IsTrue(equalsResult);
            Assert.IsTrue(operatorResult);
        }

        [Test]
        public void WhenComparingPuzzleCellWithEmptyCell_AndValuesAreEqual_ThenReturnsTrue()
        {
            // Arrange
            var emptyCell = new EmptyCell();
            var puzzleCell = new PuzzleCell(emptyCell);

            // Act
            var equalsResult = puzzleCell.Equals(emptyCell);
            var operatorResult = puzzleCell == emptyCell;

            // Assert
            Assert.IsTrue(equalsResult);
            Assert.IsTrue(operatorResult);
        }

        [Test]
        public void WhenComparingPuzzleCellWithRegularCell_AndValuesAreEqual_ThenReturnsTrue()
        {
            // Arrange
            var regularCell = new RegularCell();
            var puzzleCell = new PuzzleCell(regularCell);

            // Act
            var equalsResult = puzzleCell.Equals(regularCell);
            var operatorResult = puzzleCell == regularCell;

            // Assert
            Assert.IsTrue(equalsResult);
            Assert.IsTrue(operatorResult);
        }
    }
}