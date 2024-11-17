using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Moq;
using WSyncPro.Core.Services;
using WSyncPro.Models.Content;
using WSyncPro.Models.Config;
using WSyncPro.Models.Data;
using WSyncPro.Models.Enum.ReRenderEnumCollection;
using Xunit;

namespace WSyncPro.Test.Unit.Core
{
    public class ReRenderServiceUnitTest
    {
        private readonly Mock<IDirectoryScannerService> _mockDirectoryScannerService;
        private readonly WEnv _envCache;
        private readonly ReRenderService _reRenderService;

        public ReRenderServiceUnitTest()
        {
            _mockDirectoryScannerService = new Mock<IDirectoryScannerService>();
            _envCache = new WEnv();

            // Mock File.Exists function
            Func<string, bool> fakeFileExists = path => path == @"C:\FakePath\HandBrakeCLI.exe" || path == @"C:\Videos\InputVideo.mp4";

            // Set up a fake HandBrakeCLI path for testing
            Environment.SetEnvironmentVariable("HANDBRAKE_CLI", @"C:\FakePath\HandBrakeCLI.exe");

            _reRenderService = new ReRenderService(_mockDirectoryScannerService.Object, _envCache, fakeFileExists);
        }



        [Fact]
        public async Task ReRender_ShouldThrowIfHandBrakePathNotSet()
        {
            // Arrange
            Environment.SetEnvironmentVariable("HANDBRAKE_CLI", null); // Unset the environment variable

            var job = new RenderJob
            {
                SrcDirectory = @"C:\Videos\InputVideo.mp4",
                DstDirectory = @"C:\Videos",
                VideoType = VideoType.MKV
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _reRenderService.ReRender(job));
        }

        [Fact]
        public async Task ReRender_ShouldThrowIfSourcePathDoesNotExist()
        {
            // Arrange
            var job = new RenderJob
            {
                SrcDirectory = @"C:\NonExistentPath",
                DstDirectory = @"C:\Videos",
                VideoType = VideoType.MKV
            };

            // Act & Assert
            await Assert.ThrowsAsync<DirectoryNotFoundException>(() => _reRenderService.ReRender(job));
        }
    }
}
