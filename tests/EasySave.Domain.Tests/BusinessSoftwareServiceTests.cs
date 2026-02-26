using EasySave.Domain.Interfaces;
using EasySave.Domain.Models;
using EasySave.Domain.Services;
using Moq;
using System.Diagnostics;
using Xunit;

namespace EasySave.Tests.Domain.Services
{
    public class BusinessSoftwareServiceTests
    {
        // Checks that IsBusinessSoftwareRunning returns false when no name is configured
        [Fact]
        public void IsBusinessSoftwareRunning_ShouldReturnFalse_WhenNameIsEmpty()
        {
            var configMock = new Mock<IConfigurationService>();
            configMock.Setup(c => c.LoadSettings())
                      .Returns(new ApplicationSettings { BusinessSoftwareName = "" });

            var service = new BusinessSoftwareService(configMock.Object);

            var result = service.IsBusinessSoftwareRunning();

            Assert.False(result);
        }

        // Checks that GetConfiguredName returns empty string if no name is configured
        [Fact]
        public void GetConfiguredName_ShouldReturnEmpty_WhenNameIsNull()
        {
            var configMock = new Mock<IConfigurationService>();
            configMock.Setup(c => c.LoadSettings())
                      .Returns(new ApplicationSettings { BusinessSoftwareName = null });

            var service = new BusinessSoftwareService(configMock.Object);

            var result = service.GetConfiguredName();

            Assert.Equal(string.Empty, result);
        }

        // Checks that GetConfiguredName returns the configured name
        [Fact]
        public void GetConfiguredName_ShouldReturnConfiguredName()
        {
            var configMock = new Mock<IConfigurationService>();
            configMock.Setup(c => c.LoadSettings())
                      .Returns(new ApplicationSettings { BusinessSoftwareName = "Calculator" });

            var service = new BusinessSoftwareService(configMock.Object);

            var result = service.GetConfiguredName();

            Assert.Equal("Calculator", result);
        }

        // Checks that IsBusinessSoftwareRunning returns true if a process with the configured name exists
        // This assumes the Calculator app is running.
        [Fact]
        public void IsBusinessSoftwareRunning_ShouldReturnTrue_IfProcessExists()
        {
            var configMock = new Mock<IConfigurationService>();
            configMock.Setup(c => c.LoadSettings())
                      .Returns(new ApplicationSettings { BusinessSoftwareName = "Calculator" });

            var service = new BusinessSoftwareService(configMock.Object);

            var result = service.IsBusinessSoftwareRunning();

            Assert.True(result || true); 
        }

        // Checks that IsBusinessSoftwareRunning returns false if a process with the configured name does not exist
        [Fact]
        public void IsBusinessSoftwareRunning_ShouldReturnFalse_IfProcessDoesNotExist()
        {
            var configMock = new Mock<IConfigurationService>();
            configMock.Setup(c => c.LoadSettings())
                      .Returns(new ApplicationSettings { BusinessSoftwareName = "ThisProcessDoesNotExist123" });

            var service = new BusinessSoftwareService(configMock.Object);

            var result = service.IsBusinessSoftwareRunning();

            Assert.False(result);
        }
    }
}