using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Moq;
using WSyncPro.Core.Services;
using WSyncPro.Models.Files;
using Xunit;

namespace WSyncPro.Test.Unit.Core
{
    public class DirectoryManagerUnitTest
    {
        private readonly DirectoryManager _directoryManager;

        public DirectoryManagerUnitTest()
        {
            _directoryManager = new DirectoryManager();
        }

        [Fact]
        public async Task GetItemById_ItemExists_ReturnsItem()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var item = new WBaseItem
            {
                Id = itemId,
                Name = "TestItem",
                Path = "/test/path",
                Created = DateTime.Now,
                LastUpdated = DateTime.Now
            };

            // Using reflection to add the item to private cache
            AddToCacheById(item);

            // Act
            var result = await _directoryManager.GetItemById(itemId);

            // Assert
            Assert.Equal(item, result);
        }

        [Fact]
        public async Task GetItemById_ItemDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await _directoryManager.GetItemById(nonExistentId);
            });
        }

        [Fact]
        public async Task GetItemByName_ItemExists_ReturnsItem()
        {
            // Arrange
            var itemName = "TestItem";
            var item = new WBaseItem
            {
                Id = Guid.NewGuid(),
                Name = itemName,
                Path = "/test/path",
                Created = DateTime.Now,
                LastUpdated = DateTime.Now
            };

            AddToCacheByName(item);

            // Act
            var result = await _directoryManager.GetItemByName(itemName);

            // Assert
            Assert.Equal(item, result);
        }

        [Fact]
        public async Task GetItemByName_ItemDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var nonExistentName = "NonExistentItem";

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await _directoryManager.GetItemByName(nonExistentName);
            });
        }

        [Fact]
        public async Task GetAllRootDirectories_NoRootDirectories_ReturnsEmptyList()
        {
            // Act
            var result = await _directoryManager.GetAllRootDirectories();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task ScanDirectory_ValidPath_ReturnsDirectoryWithNestedFilesAndDirectories()
        {
            // Arrange
            var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "TestRoot");
            CreateTestFiles(rootPath);

            // Act
            var result = await _directoryManager.ScanDirectory(rootPath);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TestRoot", result.Name);
            Assert.Equal(3, result.Items.Count); // 2 files, 1 subdirectory

            var file1 = (WFile)result.Items.Find(i => i.Name == "file1.txt");
            Assert.NotNull(file1);
            Assert.Equal("file1.txt", file1.Name);

            var file2 = (WFile)result.Items.Find(i => i.Name == "file2.log");
            Assert.NotNull(file2);
            Assert.Equal("file2.log", file2.Name);

            var subdirectory = (WDirectory)result.Items.Find(i => i.Name == "SubDir");
            Assert.NotNull(subdirectory);
            Assert.Equal("SubDir", subdirectory.Name);
            Assert.Equal(2, subdirectory.Items.Count);

            var nestedFile1 = (WFile)subdirectory.Items.Find(i => i.Name == "nested1.txt");
            Assert.NotNull(nestedFile1);
            Assert.Equal("nested1.txt", nestedFile1.Name);

            var nestedFile2 = (WFile)subdirectory.Items.Find(i => i.Name == "nested2.md");
            Assert.NotNull(nestedFile2);
            Assert.Equal("nested2.md", nestedFile2.Name);

            // Cleanup
            DeleteTestFiles(rootPath);
        }

        [Fact]
        public async Task ScanDirectory_NonExistentPath_ThrowsDirectoryNotFoundException()
        {
            // Arrange
            var invalidPath = Path.Combine(Directory.GetCurrentDirectory(), "NonExistentDirectory");

            // Act & Assert
            await Assert.ThrowsAsync<DirectoryNotFoundException>(async () =>
            {
                await _directoryManager.ScanDirectory(invalidPath);
            });
        }

        private void AddToCacheById(WBaseItem item)
        {
            var cacheByIdField = typeof(DirectoryManager).GetField("_cacheById", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var cacheById = (Dictionary<Guid, WBaseItem>)cacheByIdField.GetValue(_directoryManager);
            cacheById[item.Id] = item;
        }

        private void AddToCacheByName(WBaseItem item)
        {
            var cacheByNameField = typeof(DirectoryManager).GetField("_cacheByName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var cacheByName = (Dictionary<string, WBaseItem>)cacheByNameField.GetValue(_directoryManager);
            cacheByName[item.Name] = item;
        }

        private void CreateTestFiles(string rootPath)
        {
            Directory.CreateDirectory(rootPath);
            File.Create(Path.Combine(rootPath, "file1.txt")).Dispose();
            File.Create(Path.Combine(rootPath, "file2.log")).Dispose();

            var subDirPath = Path.Combine(rootPath, "SubDir");
            Directory.CreateDirectory(subDirPath);
            File.Create(Path.Combine(subDirPath, "nested1.txt")).Dispose();
            File.Create(Path.Combine(subDirPath, "nested2.md")).Dispose();
        }

        private void DeleteTestFiles(string rootPath)
        {
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, true);
            }
        }
    }
}