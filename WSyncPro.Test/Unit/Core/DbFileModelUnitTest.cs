using System;
using System.Collections.Generic;
using System.Linq;
using WSyncPro.Core.Models;
using Xunit;

namespace WSyncPro.Test.Unit.Core
{
    public class DbFileModelUnitTest
    {
        [Fact]
        public void GetHistory_ShouldReturnEmptyList_WhenHistoryIsEmpty()
        {
            // Arrange
            var dbFile = new DbFile { History = string.Empty };

            // Act
            var history = dbFile.GetHistory();

            // Assert
            Assert.NotNull(history);
            Assert.Empty(history);
        }

        [Fact]
        public void GetHistory_ShouldParseHistoryStringIntoList()
        {
            // Arrange
            var dbFile = new DbFile
            {
                History = "Entry1||Entry2||Entry3"
            };

            // Act
            var history = dbFile.GetHistory();

            // Assert
            Assert.NotNull(history);
            Assert.Equal(3, history.Count);
            Assert.Contains("Entry1", history);
            Assert.Contains("Entry2", history);
            Assert.Contains("Entry3", history);
        }

        [Fact]
        public void SetHistory_ShouldUpdateHistoryString()
        {
            // Arrange
            var dbFile = new DbFile();
            var newHistory = new List<string> { "NewEntry1", "NewEntry2" };

            // Act
            dbFile.SetHistory(newHistory);

            // Assert
            Assert.Equal("NewEntry1||NewEntry2", dbFile.History);
        }

        [Fact]
        public void SetHistory_ShouldClearHistory_WhenListIsEmpty()
        {
            // Arrange
            var dbFile = new DbFile { History = "ExistingEntry" };
            var newHistory = new List<string>();

            // Act
            dbFile.SetHistory(newHistory);

            // Assert
            Assert.Equal(string.Empty, dbFile.History);
        }

        [Fact]
        public void SetHistory_ShouldClearHistory_WhenListIsNull()
        {
            // Arrange
            var dbFile = new DbFile { History = "ExistingEntry" };

            // Act
            dbFile.SetHistory(null);

            // Assert
            Assert.Equal(string.Empty, dbFile.History);
        }

        [Fact]
        public void AddToHistory_ShouldAddNewEntry()
        {
            // Arrange
            var dbFile = new DbFile { History = "ExistingEntry" };
            var newEntry = "NewEntry";

            // Act
            dbFile.AddToHistory(newEntry);

            // Assert
            var history = dbFile.GetHistory();
            Assert.Equal(2, history.Count);
            Assert.Contains("ExistingEntry", history);
            Assert.Contains("NewEntry", history);
        }

        [Fact]
        public void AddToHistory_ShouldHandleEmptyHistory()
        {
            // Arrange
            var dbFile = new DbFile { History = string.Empty };
            var newEntry = "NewEntry";

            // Act
            dbFile.AddToHistory(newEntry);

            // Assert
            var history = dbFile.GetHistory();
            Assert.Single(history);
            Assert.Contains("NewEntry", history);
        }

        [Fact]
        public void AddToHistory_ShouldIgnoreNullOrWhitespaceEntries()
        {
            // Arrange
            var dbFile = new DbFile { History = "ExistingEntry" };

            // Act
            dbFile.AddToHistory(null);
            dbFile.AddToHistory("");
            dbFile.AddToHistory("   "); // Whitespace only

            // Assert
            var history = dbFile.GetHistory();
            Assert.Single(history);
            Assert.Contains("ExistingEntry", history);
        }

        [Fact]
        public void GetHistory_ShouldReturnEntriesInOrder()
        {
            // Arrange
            var dbFile = new DbFile
            {
                History = "FirstEntry||SecondEntry||ThirdEntry"
            };

            // Act
            var history = dbFile.GetHistory();

            // Assert
            Assert.Equal(new List<string> { "FirstEntry", "SecondEntry", "ThirdEntry" }, history);
        }
    }
}
