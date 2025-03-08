using NUnit.Framework;

namespace Appegy.Union.Generator.Tests.Results
{
    [TestFixture]
    public class PuzzleCellInequalityOperatorTests
    {
        [Test]
        public void WhenComparingPuzzleCellWithPuzzleCell_AndValuesAreEqual_ThenNotEqualsReturnsFalse()
        {
            // Arrange
            var voidCell = new VoidCell();
            var puzzleCell1 = new PuzzleCell(voidCell);
            var puzzleCell2 = new PuzzleCell(voidCell);

            // Act
            var operatorResult = puzzleCell1 != puzzleCell2;

            // Assert
            Assert.IsFalse(operatorResult);
        }

        [Test]
        public void WhenComparingPuzzleCellWithPuzzleCell_AndValuesAreNotEqual_ThenNotEqualsReturnsTrue()
        {
            // Arrange
            var voidCell = new VoidCell();
            var emptyCell = new EmptyCell();
            var puzzleCell1 = new PuzzleCell(voidCell);
            var puzzleCell2 = new PuzzleCell(emptyCell);

            // Act
            var operatorResult = puzzleCell1 != puzzleCell2;

            // Assert
            Assert.IsTrue(operatorResult);
        }

        [Test]
        public void WhenComparingPuzzleCellWithVoidCell_AndValuesAreEqual_ThenNotEqualsReturnsFalse()
        {
            // Arrange
            var voidCell = new VoidCell();
            var puzzleCell = new PuzzleCell(voidCell);

            // Act
            var operatorResult = puzzleCell != voidCell;

            // Assert
            Assert.IsFalse(operatorResult);
        }

        [Test]
        public void WhenComparingPuzzleCellWithVoidCell_AndValuesAreNotEqual_ThenNotEqualsReturnsTrue()
        {
            // Arrange
            var voidCell = new VoidCell();
            var emptyCell = new EmptyCell();
            var puzzleCell = new PuzzleCell(emptyCell);

            // Act
            var operatorResult = puzzleCell != voidCell;

            // Assert
            Assert.IsTrue(operatorResult);
        }

        [Test]
        public void WhenComparingPuzzleCellWithEmptyCell_AndValuesAreEqual_ThenNotEqualsReturnsFalse()
        {
            // Arrange
            var emptyCell = new EmptyCell();
            var puzzleCell = new PuzzleCell(emptyCell);

            // Act
            var operatorResult = puzzleCell != emptyCell;

            // Assert
            Assert.IsFalse(operatorResult);
        }

        [Test]
        public void WhenComparingPuzzleCellWithEmptyCell_AndValuesAreNotEqual_ThenNotEqualsReturnsTrue()
        {
            // Arrange
            var emptyCell = new EmptyCell();
            var regularCell = new RegularCell(1); // Unique ID
            var puzzleCell = new PuzzleCell(regularCell);

            // Act
            var operatorResult = puzzleCell != emptyCell;

            // Assert
            Assert.IsTrue(operatorResult);
        }

        [Test]
        public void WhenComparingPuzzleCellWithRegularCell_AndValuesAreEqual_ThenNotEqualsReturnsFalse()
        {
            // Arrange
            var regularCell = new RegularCell(1); // Same ID
            var puzzleCell = new PuzzleCell(regularCell);

            // Act
            var operatorResult = puzzleCell != regularCell;

            // Assert
            Assert.IsFalse(operatorResult);
        }

        [Test]
        public void WhenComparingPuzzleCellWithRegularCell_AndValuesAreNotEqual_ThenNotEqualsReturnsTrue()
        {
            // Arrange
            var regularCell1 = new RegularCell(1); // Different ID
            var regularCell2 = new RegularCell(2); // Different ID
            var puzzleCell = new PuzzleCell(regularCell1);

            // Act
            var operatorResult = puzzleCell != regularCell2;

            // Assert
            Assert.IsTrue(operatorResult);
        }
    }
}