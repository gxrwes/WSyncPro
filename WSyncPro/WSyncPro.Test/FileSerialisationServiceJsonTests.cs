using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using WSyncPro.Models;
using WSyncPro.Test.TestClasses;
using WSyncPro.Core.Services;
using Xunit;

namespace WSyncPro.Test
{
    public class FileSerialisationServiceJsonTests : IDisposable
    {
        private readonly string _tempDir;
        private readonly string _testFilePath;
        private readonly FileSerialisationServiceJson _serializationService;

        public FileSerialisationServiceJsonTests()
        {
            // Create a unique temporary directory for testing
            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDir);

            // Define the test JSON file path
            _testFilePath = Path.Combine(_tempDir, "test.json");

            // Initialize the serialization service
            _serializationService = new FileSerialisationServiceJson();
        }

        public void Dispose()
        {
            // Clean up the temporary directory after tests
            if (Directory.Exists(_tempDir))
            {
                try
                {
                    Directory.Delete(_tempDir, true);
                }
                catch
                {
                    // Ignore any exceptions during cleanup
                }
            }
        }

        [Fact]
        public async Task GetFileAsClassAsync_ShouldDeserializeDeserialisationTestClassesFromJsonFile()
        {
            // Arrange
            var testClass = new DeserialisationTestClasses
            {
                Title = "Test Title",
                Count = 5,
                Children = new List<ChildClass>
                {
                    new ChildClass { Name = "Child1", Tags = new List<string> { "Tag1", "Tag2" } },
                    new ChildClass { Name = "Child2", Tags = new List<string> { "Tag3", "Tag4" } }
                },
                Nested = new NestedClass
                {
                    Timestamp = DateTime.UtcNow,
                    SubNestedItems = new List<SubNestedClass>
                    {
                        new SubNestedClass { Detail = "Detail1", Value = 100 },
                        new SubNestedClass { Detail = "Detail2", Value = 200 }
                    }
                }
            };

            // Act
            bool saveResult = await _serializationService.SaveClassToFileAsync(_testFilePath, testClass);
            saveResult.Should().BeTrue();

            DeserialisationTestClasses deserializedTestClass = await _serializationService.GetFileAsClassAsync<DeserialisationTestClasses>(_testFilePath);

            // Assert
            deserializedTestClass.Should().NotBeNull();
            deserializedTestClass.Title.Should().Be("Test Title");
            deserializedTestClass.Count.Should().Be(5);
            deserializedTestClass.Children.Should().HaveCount(2);
            deserializedTestClass.Children[0].Name.Should().Be("Child1");
            deserializedTestClass.Children[0].Tags.Should().Contain(new[] { "Tag1", "Tag2" });
            deserializedTestClass.Nested.Should().NotBeNull();
            deserializedTestClass.Nested.SubNestedItems.Should().HaveCount(2);
            deserializedTestClass.Nested.SubNestedItems[0].Detail.Should().Be("Detail1");
            deserializedTestClass.Nested.SubNestedItems[0].Value.Should().Be(100);
        }

        [Fact]
        public async Task GetFileAsClassAsync_ShouldDeserializeJobObjectFromJsonFile()
        {
            // Arrange
            var job = new Job
            {
                Id = "J001",
                Name = "Backup",
                Created = DateTime.UtcNow.AddDays(-5),
                LastRun = DateTime.UtcNow,
                Description = "Backup files to remote server",
                IsEnabled = true
            };

            // Act
            bool saveResult = await _serializationService.SaveClassToFileAsync(_testFilePath, job);
            saveResult.Should().BeTrue();

            Job deserializedJob = await _serializationService.GetFileAsClassAsync<Job>(_testFilePath);

            // Assert
            deserializedJob.Should().NotBeNull();
            deserializedJob.Id.Should().Be("J001");
            deserializedJob.Name.Should().Be("Backup");
            deserializedJob.Description.Should().Be("Backup files to remote server");
            deserializedJob.IsEnabled.Should().BeTrue();
        }

        [Fact]
        public async Task GetFileAsClassAsync_ShouldDeserializeSyncJobObjectWithInheritance()
        {
            // Arrange
            var syncJob = new SyncJob
            {
                Id = "SJ001",
                Name = "File Sync",
                Created = DateTime.UtcNow.AddDays(-10),
                LastRun = DateTime.UtcNow,
                Description = "Synchronize files between directories",
                IsEnabled = true,
                SrcDirectory = @"C:\Source",
                DstDirectory = @"D:\Destination",
                FilesToSync = 100,
                TotalFilesSynced = 95,
                FailedFiles = 5,
                InclWilcardString = new List<string> { "*.txt", "*.docx" },
                ExclWildcardString = new List<string> { "*.tmp", "*.log" }
            };

            // Act
            bool saveResult = await _serializationService.SaveClassToFileAsync(_testFilePath, syncJob);
            saveResult.Should().BeTrue();

            SyncJob deserializedSyncJob = await _serializationService.GetFileAsClassAsync<SyncJob>(_testFilePath);

            // Assert
            deserializedSyncJob.Should().NotBeNull();
            deserializedSyncJob.Should().BeOfType<SyncJob>();
            deserializedSyncJob.Id.Should().Be("SJ001");
            deserializedSyncJob.Name.Should().Be("File Sync");
            deserializedSyncJob.SrcDirectory.Should().Be(@"C:\Source");
            deserializedSyncJob.DstDirectory.Should().Be(@"D:\Destination");
            deserializedSyncJob.FilesToSync.Should().Be(100);
            deserializedSyncJob.TotalFilesSynced.Should().Be(95);
            deserializedSyncJob.FailedFiles.Should().Be(5);
            deserializedSyncJob.InclWilcardString.Should().Contain(new[] { "*.txt", "*.docx" });
            deserializedSyncJob.ExclWildcardString.Should().Contain(new[] { "*.tmp", "*.log" });
        }

        [Fact]
        public async Task GetFileAsClassAsync_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
        {
            // Arrange
            string nonExistentFilePath = Path.Combine(_tempDir, "nonexistent.json");

            // Act
            Func<Task> act = async () => await _serializationService.GetFileAsClassAsync<Job>(nonExistentFilePath);

            // Assert
            await act.Should().ThrowAsync<FileNotFoundException>()
                .WithMessage($"*{nonExistentFilePath}*");
        }

        [Fact]
        public async Task SaveClassToFileAsync_ShouldHandleEmptyNestedClassesAndLists()
        {
            // Arrange
            var testClass = new DeserialisationTestClasses
            {
                Title = "Empty Test",
                Count = 0,
                Children = new List<ChildClass>(), // Empty list
                Nested = new NestedClass
                {
                    Timestamp = DateTime.UtcNow,
                    SubNestedItems = new List<SubNestedClass>() // Empty list
                }
            };

            // Act
            bool result = await _serializationService.SaveClassToFileAsync(_testFilePath, testClass);

            // Assert
            result.Should().BeTrue();
            File.Exists(_testFilePath).Should().BeTrue();

            DeserialisationTestClasses deserializedTestClass = await _serializationService.GetFileAsClassAsync<DeserialisationTestClasses>(_testFilePath);

            deserializedTestClass.Should().NotBeNull();
            deserializedTestClass.Title.Should().Be("Empty Test");
            deserializedTestClass.Count.Should().Be(0);
            deserializedTestClass.Children.Should().BeEmpty();
            deserializedTestClass.Nested.Should().NotBeNull();
            deserializedTestClass.Nested.SubNestedItems.Should().BeEmpty();
        }

        [Fact]
        public async Task SaveClassToFileAsync_ShouldReturnFalse_WhenSerializationFails()
        {
            // Arrange
            var job = new Job
            {
                Id = "J002",
                Name = "Cleanup",
                Created = DateTime.UtcNow.AddDays(-3),
                LastRun = DateTime.UtcNow,
                Description = "Clean up temporary files",
                IsEnabled = true
            };

            // Create a file that will act as a directory
            string invalidDirPath = Path.Combine(_tempDir, "testfile.json");
            File.WriteAllText(invalidDirPath, "This is a file, not a directory.");

            // Now, attempt to serialize to "testfile.json\file.json", which should fail
            string invalidFilePath = Path.Combine(invalidDirPath, "file.json");

            // Act
            bool result = await _serializationService.SaveClassToFileAsync(invalidFilePath, job);

            // Assert
            result.Should().BeFalse();
            File.Exists(invalidFilePath).Should().BeFalse();
        }


        [Fact]
        public async Task SaveClassToFileAsync_ShouldSaveDeserialisationTestClassesToJsonFile()
        {
            // Arrange
            var testClass = new DeserialisationTestClasses
            {
                Title = "Serialization Test",
                Count = 2,
                Children = new List<ChildClass>
                {
                    new ChildClass { Name = "Child1", Tags = new List<string> { "TagA", "TagB" } },
                    new ChildClass { Name = "Child2", Tags = new List<string> { "TagC", "TagD" } }
                },
                Nested = new NestedClass
                {
                    Timestamp = DateTime.UtcNow,
                    SubNestedItems = new List<SubNestedClass>
                    {
                        new SubNestedClass { Detail = "SubDetail1", Value = 10 },
                        new SubNestedClass { Detail = "SubDetail2", Value = 20 }
                    }
                }
            };

            // Act
            bool result = await _serializationService.SaveClassToFileAsync(_testFilePath, testClass);

            // Assert
            result.Should().BeTrue();
            File.Exists(_testFilePath).Should().BeTrue();

            string jsonContent = await File.ReadAllTextAsync(_testFilePath);
            jsonContent.Should().Contain("Serialization Test");
            jsonContent.Should().Contain("Child1");
            jsonContent.Should().Contain("SubDetail1");
        }

        [Fact]
        public async Task SaveClassToFileAsync_ShouldSerializeJobObjectToJsonFile()
        {
            // Arrange
            var job = new Job
            {
                Id = "J003",
                Name = "Monitoring",
                Created = DateTime.UtcNow.AddDays(-7),
                LastRun = DateTime.UtcNow,
                Description = "Monitor system performance",
                IsEnabled = true
            };

            // Act
            bool result = await _serializationService.SaveClassToFileAsync(_testFilePath, job);

            // Assert
            result.Should().BeTrue();
            File.Exists(_testFilePath).Should().BeTrue();

            string jsonContent = await File.ReadAllTextAsync(_testFilePath);
            jsonContent.Should().Contain("Monitoring");
            jsonContent.Should().Contain("Monitor system performance");
        }

        [Fact]
        public async Task SaveClassToFileAsync_ShouldSerializeSyncJobObjectWithInheritance()
        {
            // Arrange
            var syncJob = new SyncJob
            {
                Id = "SJ002",
                Name = "Data Sync",
                Created = new DateTime(2024, 8, 30, 11, 29, 58, 863, DateTimeKind.Utc).AddTicks(1028),
                LastRun = new DateTime(2024, 9, 19, 6, 29, 58, 863, DateTimeKind.Utc).AddTicks(1342),
                Description = "Synchronize data between systems",
                IsEnabled = true,
                SrcDirectory = @"C:\Source",
                DstDirectory = @"D:\Destination",
                FilesToSync = 100,
                TotalFilesSynced = 95,
                FailedFiles = 5,
                InclWilcardString = new List<string> { "*.txt", "*.docx" },
                ExclWildcardString = new List<string> { "*.tmp", "*.log" }
            };

            // Act
            bool saveResult = await _serializationService.SaveClassToFileAsync(_testFilePath, syncJob);
            saveResult.Should().BeTrue();
            File.Exists(_testFilePath).Should().BeTrue();

            string jsonContent = await File.ReadAllTextAsync(_testFilePath);

            // Assert
            // Deserialize back to SyncJob and verify properties
            SyncJob deserializedSyncJob = await _serializationService.GetFileAsClassAsync<SyncJob>(_testFilePath);
            deserializedSyncJob.Should().NotBeNull();
            deserializedSyncJob.SrcDirectory.Should().Be(@"C:\Source");
            deserializedSyncJob.DstDirectory.Should().Be(@"D:\Destination");
            deserializedSyncJob.InclWilcardString.Should().Contain(new[] { "*.txt", "*.docx" });
        }
    }
}
