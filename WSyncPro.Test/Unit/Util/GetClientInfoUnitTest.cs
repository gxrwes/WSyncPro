using System;
using WSyncPro.Util.System;
using WSyncPro.Models.User;
using Xunit;

namespace WSyncPro.Test.Unit.Util
{
    public class GetClientInfoUnitTest
    {
        [Fact]
        [Trait("Category", "Unit Test")] // Grouping this test under "Unit Test" category.
        public void GetClientSystemInformation_ReturnsValidClient()
        {
            // Act
            var client = GetClientInfo.GetClientSystemInformation();

            // Assert
            Assert.NotNull(client);
            Assert.NotEqual(Guid.Empty, client.Id);
            Assert.False(string.IsNullOrWhiteSpace(client.Name));
            Assert.False(string.IsNullOrWhiteSpace(client.MachineId));
            Assert.False(string.IsNullOrWhiteSpace(client.Ip));
            Assert.NotNull(client.OperatingSystem);
        }

        [Fact]
        [Trait("Category", "Unit Test")] // Grouping this test under "Unit Test" category.
        public void GetMachineId_ReturnsNonEmptyString()
        {
            // Act
            var machineId = GetClientInfo.GetMachineId();

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(machineId));
        }

        [Fact]
        [Trait("Category", "Unit Test")] // Grouping this test under "Unit Test" category.
        public void GetLocalIPAddress_ReturnsValidIP()
        {
            // Act
            var ip = GetClientInfo.GetLocalIPAddress();

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(ip));
            Assert.Matches(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b", ip); // Basic IPv4 pattern match
        }

        [Fact]
        [Trait("Category", "Unit Test")] // Grouping this test under "Unit Test" category.
        public void GetOperatingSystemInfo_ReturnsValidOperatingSystem()
        {
            // Act
            var os = GetClientInfo.GetOperatingSystemInfo();

            // Assert
            Assert.NotNull(os);
            Assert.False(string.IsNullOrWhiteSpace(os.VersionString));
        }
    }
}
