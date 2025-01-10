using NUnit.Framework;
using System;
using System.Collections.Generic;
using WSyncPro.Models.Files;
using WSyncPro.Models.Import;

namespace WSyncPro.Test.Unit
{
    [TestFixture]
    internal class ImportPathBuilderUnitTest
    {
        [Test]
        public void GetBuiltPath_BuildsCorrectPath()
        {
            // Arrange
            var importPathBuilder = new ImportPathBuilder();
            var originalFile = new WFile
            {
                Id = Guid.Empty,
                Name = "example.txt"
            };
            var pathTypes = new List<ImportPathType>
            {
                ImportPathType.DateYear,
                ImportPathType.DateMonth,
                ImportPathType.FileName
            };
            int counter = 42;

            // Act
            var result = importPathBuilder.GetBuiltPath(originalFile, pathTypes, counter);

            // Assert
            var expected = $"{DateTime.Now.Year}/{DateTime.Now.Month:D2}/example.txt";
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetBuiltPath_NullFile_ThrowsArgumentNullException()
        {
            // Arrange
            var importPathBuilder = new ImportPathBuilder();
            List<ImportPathType> pathTypes = new List<ImportPathType> { ImportPathType.FileName };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => importPathBuilder.GetBuiltPath(null, pathTypes, 0));
        }

        [Test]
        public void GetBuiltPath_EmptyPathBuilder_ReturnsEmptyString()
        {
            // Arrange
            var importPathBuilder = new ImportPathBuilder();
            var originalFile = new WFile
            {
                Id = Guid.Empty,
                Name = "example.txt"
            };
            var pathTypes = new List<ImportPathType>();

            // Act
            var result = importPathBuilder.GetBuiltPath(originalFile, pathTypes, 0);

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void GetBuiltPath_CounterIncluded_ReturnsPathWithCounter()
        {
            // Arrange
            var importPathBuilder = new ImportPathBuilder();
            var originalFile = new WFile
            {
                Id = Guid.Empty,
                Name = "example.txt"
            };
            var pathTypes = new List<ImportPathType>
            {
                ImportPathType.Counter,
                ImportPathType.FileName
            };
            int counter = 42;

            // Act
            var result = importPathBuilder.GetBuiltPath(originalFile, pathTypes, counter);

            // Assert
            var expected = "[42]/example.txt";
            Assert.AreEqual(expected, result);
        }
    }
}
