using System;
using NUnit.Framework;

namespace Appegy.Union.Generator.Tests.Results
{
    [TestFixture]
    public class PuzzleCellPropertyTests
    {
        [Test]
        public void WhenGettingVoidCell_AndTypeIsVoidCell_ThenReturnsVoidCell()
        {
            // Arrange
            var voidCell = new VoidCell();
            var puzzleCell = new PuzzleCell(voidCell);

            // Act
            var result = puzzleCell.VoidCell;

            // Assert
            Assert.AreEqual(voidCell, result);
        }

        [Test]
        public void WhenGettingVoidCell_AndTypeIsNotVoidCell_ThenThrowsException()
        {
            // Arrange
            var emptyCell = new EmptyCell();
            var puzzleCell = new PuzzleCell(emptyCell);

            // Act & Assert
            Assert.Throws<Exception>(() => { _ = puzzleCell.VoidCell; });
        }

        [Test]
        public void WhenSettingVoidCell_ThenTypeIsVoidCell()
        {
            // Arrange
            var puzzleCell = new PuzzleCell();
            var voidCell = new VoidCell();

            // Act
            puzzleCell.VoidCell = voidCell;

            // Assert
            Assert.AreEqual(PuzzleCell.Kind.VoidCell, puzzleCell.Type);
            Assert.AreEqual(voidCell, puzzleCell.VoidCell);
        }

        [Test]
        public void WhenGettingEmptyCell_AndTypeIsEmptyCell_ThenReturnsEmptyCell()
        {
            // Arrange
            var emptyCell = new EmptyCell();
            var puzzleCell = new PuzzleCell(emptyCell);

            // Act
            var result = puzzleCell.EmptyCell;

            // Assert
            Assert.AreEqual(emptyCell, result);
        }

        [Test]
        public void WhenGettingEmptyCell_AndTypeIsNotEmptyCell_ThenThrowsException()
        {
            // Arrange
            var regularCell = new RegularCell();
            var puzzleCell = new PuzzleCell(regularCell);

            // Act & Assert
            Assert.Throws<Exception>(() => { _ = puzzleCell.EmptyCell; });
        }

        [Test]
        public void WhenSettingEmptyCell_ThenTypeIsEmptyCell()
        {
            // Arrange
            var puzzleCell = new PuzzleCell();
            var emptyCell = new EmptyCell();

            // Act
            puzzleCell.EmptyCell = emptyCell;

            // Assert
            Assert.AreEqual(PuzzleCell.Kind.EmptyCell, puzzleCell.Type);
            Assert.AreEqual(emptyCell, puzzleCell.EmptyCell);
        }

        [Test]
        public void WhenGettingRegularCell_AndTypeIsRegularCell_ThenReturnsRegularCell()
        {
            // Arrange
            var regularCell = new RegularCell();
            var puzzleCell = new PuzzleCell(regularCell);

            // Act
            var result = puzzleCell.RegularCell;

            // Assert
            Assert.AreEqual(regularCell, result);
        }

        [Test]
        public void WhenGettingRegularCell_AndTypeIsNotRegularCell_ThenThrowsException()
        {
            // Arrange
            var voidCell = new VoidCell();
            var puzzleCell = new PuzzleCell(voidCell);

            // Act & Assert
            Assert.Throws<Exception>(() => { _ = puzzleCell.RegularCell; });
        }

        [Test]
        public void WhenSettingRegularCell_ThenTypeIsRegularCell()
        {
            // Arrange
            var puzzleCell = new PuzzleCell();
            var regularCell = new RegularCell();

            // Act
            puzzleCell.RegularCell = regularCell;

            // Assert
            Assert.AreEqual(PuzzleCell.Kind.RegularCell, puzzleCell.Type);
            Assert.AreEqual(regularCell, puzzleCell.RegularCell);
        }
    }
}