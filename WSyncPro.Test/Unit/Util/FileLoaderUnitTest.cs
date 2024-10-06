using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WSyncPro.Models.Content;
using WSyncPro.Util.Files;
using Xunit;

namespace WSyncPro.Test.Unit.Util.Files
{
    public class FileLoaderUnitTest : IDisposable
    {
        private readonly string _testDirectory;
        private readonly IFileLoader _fileLoader;

        public FileLoaderUnitTest()
        {
            _fileLoader = new FileLoader();
            _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
        }

        [Fact]
        public async Task LoadFileAsStringAsync_ShouldThrowException_WhenFileDoesNotExist()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "nonExistentFile.txt");

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => _fileLoader.LoadFileAsStringAsync(filePath));
        }

        [Fact]
        public async Task LoadFileAsStringAsync_ShouldReturnFileContent()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "testFile.txt");
            var expectedContent = "Hello, World!";
            await File.WriteAllTextAsync(filePath, expectedContent);

            // Act
            var fileContent = await _fileLoader.LoadFileAsStringAsync(filePath);

            // Assert
            Assert.Equal(expectedContent, fileContent);
        }

        [Fact]
        public async Task SaveToFileAsStringAsync_ShouldWriteContentToFile()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "testFile.txt");
            var content = "Test Content";

            // Act
            await _fileLoader.SaveToFileAsStringAsync(filePath, content);

            // Assert
            Assert.True(File.Exists(filePath));
            Assert.Equal(content, await File.ReadAllTextAsync(filePath));
        }

        [Fact]
        public async Task UpdateFileWithContentAsync_ShouldAppendContentToFile()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "testFile.txt");
            var initialContent = "Initial Content";
            var additionalContent = " - Appended Content";

            await _fileLoader.SaveToFileAsStringAsync(filePath, initialContent);

            // Act
            await _fileLoader.UpdateFileWithContentAsync(filePath, additionalContent);

            // Assert
            Assert.Equal(initialContent + additionalContent, await File.ReadAllTextAsync(filePath));
        }

        [Fact]
        public void Serialize_ShouldReturnJsonString()
        {
            // Arrange
            var syncJob = new SyncJob
            {
                Id = Guid.NewGuid(),
                Name = "Test Job",
                Description = "Test job description",
                Status = WSyncPro.Models.Enum.Status.Pending,
                SrcDirectory = "C:\\Source",
                DstDirectory = "C:\\Destination"
            };

            // Act
            var serializedContent = _fileLoader.Serialize(syncJob);

            // Assert
            var expectedJson = JsonSerializer.Serialize(syncJob, new JsonSerializerOptions { WriteIndented = true, Converters = { new JsonStringEnumConverter() } });
            Assert.Equal(expectedJson, serializedContent);
        }

        [Fact]
        public async Task SaveToFileAsObjectAsync_ShouldWriteSerializedSyncJobToFile()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "syncJob.json");
            var syncJob = new SyncJob
            {
                Id = Guid.NewGuid(),
                Name = "Test Job",
                Description = "Test job description",
                Status = WSyncPro.Models.Enum.Status.Pending,
                SrcDirectory = "C:\\Source",
                DstDirectory = "C:\\Destination"
            };

            // Act
            await _fileLoader.SaveToFileAsObjectAsync(filePath, syncJob);

            // Assert
            var expectedJson = JsonSerializer.Serialize(syncJob, new JsonSerializerOptions { WriteIndented = true, Converters = { new JsonStringEnumConverter() } });
            Assert.True(File.Exists(filePath));
            Assert.Equal(expectedJson, await File.ReadAllTextAsync(filePath));
        }

        [Fact]
        public void Deserialize_ShouldReturnSyncJobFromJsonString()
        {
            // Arrange
            var jsonContent = "{ \"Id\": \"1d53a4e0-6e56-4d2f-9934-5a4ecf1a9093\", \"Name\": \"Test Job\", \"Description\": \"Test job description\", \"Status\": \"Pending\", \"SrcDirectory\": \"C:\\\\Source\", \"DstDirectory\": \"C:\\\\Destination\" }";

            // Act
            var deserializedJob = _fileLoader.Deserialize<SyncJob>(jsonContent);

            // Assert
            Assert.Equal("Test Job", deserializedJob.Name);
            Assert.Equal("Test job description", deserializedJob.Description);
            Assert.Equal("C:\\Source", deserializedJob.SrcDirectory);
            Assert.Equal("C:\\Destination", deserializedJob.DstDirectory);
            Assert.Equal(WSyncPro.Models.Enum.Status.Pending, deserializedJob.Status);
        }

        [Fact]
        public async Task LoadFileAndParseAsync_ShouldReturnSyncJobFromFile()
        {
            // Arrange
            var filePath = Path.Combine(_testDirectory, "syncJob.json");
            var expectedJob = new SyncJob
            {
                Id = Guid.NewGuid(),
                Name = "Test Job",
                Description = "Test job description",
                Status = WSyncPro.Models.Enum.Status.Pending,
                SrcDirectory = "C:\\Source",
                DstDirectory = "C:\\Destination"
            };
            var jsonContent = JsonSerializer.Serialize(expectedJob, new JsonSerializerOptions { WriteIndented = true, Converters = { new JsonStringEnumConverter() } });
            await File.WriteAllTextAsync(filePath, jsonContent);

            // Act
            var loadedJob = await _fileLoader.LoadFileAndParseAsync<SyncJob>(filePath);

            // Assert
            Assert.Equal(expectedJob.Name, loadedJob.Name);
            Assert.Equal(expectedJob.Description, loadedJob.Description);
            Assert.Equal(expectedJob.SrcDirectory, loadedJob.SrcDirectory);
            Assert.Equal(expectedJob.DstDirectory, loadedJob.DstDirectory);
            Assert.Equal(expectedJob.Status, loadedJob.Status);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
    }
}
