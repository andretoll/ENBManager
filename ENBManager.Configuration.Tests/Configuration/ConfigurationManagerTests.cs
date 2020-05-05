using ENBManager.Configuration.Services;
using ENBManager.Configuration.Tests.Stubs;
using ENBManager.TestUtils.Utils;
using NUnit.Framework;
using System;
using System.IO;

namespace ENBManager.Configuration.Tests.Configuration
{
    [TestFixture]
    public class ConfigurationManagerTests
    {
        private const string TEST_DIRECTORY = "ENBManager//test";

        [TearDown]
        public void TearDown()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            Directory.Delete(Path.Combine(appData, TEST_DIRECTORY), true);
        }

        [Test]
        public void ShouldCreateInitialSettingsFileWhenSaving()
        {
            // Arrange
            var manager = new ConfigurationManager<AppSettingsStub>();

            // Act
            manager.SaveSettings();

            // Assert
            Assert.That(File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), TEST_DIRECTORY, manager.LoadSettings().GetFilePath())));
        }

        [Test]
        public void ShouldCreateInitialSettingsFileWhenLoading()
        {
            // Arrange
            var manager = new ConfigurationManager<AppSettingsStub>();

            // Act
            _ = manager.LoadSettings();

            // Assert
            Assert.That(File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), TEST_DIRECTORY, manager.LoadSettings().GetFilePath())));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ShouldSaveAndLoadSettings(bool condition)
        {
            // Arrange
            var manager = new ConfigurationManager<AppSettingsStub>();
            var appSettings = manager.LoadSettings();
            string name = TestValues.GetRandomString();
            appSettings.Condition = condition;
            appSettings.Name = name;

            // Act
            manager.ApplySettings(appSettings);
            manager.SaveSettings();
            appSettings = manager.LoadSettings();

            // Assert
            Assert.That(appSettings.Condition, Is.EqualTo(condition));
            Assert.That(appSettings.Name, Is.EqualTo(name));
        }
    }
}
