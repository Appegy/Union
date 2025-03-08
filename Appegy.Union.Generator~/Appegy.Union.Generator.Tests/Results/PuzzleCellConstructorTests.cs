using NUnit.Framework;

namespace Appegy.Union.Generator.Tests.Results
{
    [TestFixture]
    public class PuzzleCellConstructorTests
    {
        [Test]
        public void WhenCreatingPuzzleCellWithVoidCell_ThenTypeIsVoidCell()
        {
            // Arrange
            var voidCell = new VoidCell();

            // Act
            var puzzleCell = new PuzzleCell(voidCell);

            // Assert
            Assert.AreEqual(PuzzleCell.Kind.VoidCell, puzzleCell.Type);
        }

        [Test]
        public void WhenCreatingPuzzleCellWithEmptyCell_ThenTypeIsEmptyCell()
        {
            // Arrange
            var emptyCell = new EmptyCell();

            // Act
            var puzzleCell = new PuzzleCell(emptyCell);

            // Assert
            Assert.AreEqual(PuzzleCell.Kind.EmptyCell, puzzleCell.Type);
        }

        [Test]
        public void WhenCreatingPuzzleCellWithRegularCell_ThenTypeIsRegularCell()
        {
            // Arrange
            var regularCell = new RegularCell();

            // Act
            var puzzleCell = new PuzzleCell(regularCell);

            // Assert
            Assert.AreEqual(PuzzleCell.Kind.RegularCell, puzzleCell.Type);
        }
    }
}