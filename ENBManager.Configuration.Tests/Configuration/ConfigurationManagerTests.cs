using ENBManager.Configuration.Interfaces;
using ENBManager.Configuration.Models;
using ENBManager.Configuration.Services;
using ENBManager.Configuration.Tests.Stubs;
using ENBManager.TestUtils.Utils;
using NUnit.Framework;
using System.IO;

namespace ENBManager.Configuration.Tests.Configuration
{
    [TestFixture]
    public class ConfigurationManagerTests
    {
        private IConfigurationManager<AppSettingsStub> _configManager;

        [SetUp]
        public void Setup()
        {
            _configManager = new ConfigurationManager<AppSettingsStub>();
        }

        [TearDown]
        public void TearDown()
        {
            File.SetAttributes(Path.Combine(_configManager.LoadSettings().GetFilePath()), FileAttributes.Normal);
            Directory.Delete(Path.GetDirectoryName(_configManager.LoadSettings().GetFilePath()), true);
        }

        [Test]
        public void ShouldInitializeWithAnyDerivedClass()
        {
            // Arrange
            var configManager1 = new ConfigurationManager<AppSettings>();
            var configManager2 = new ConfigurationManager<AppSettingsStub>();

            // Act
            var settings1 = configManager1.LoadSettings();
            var settings2 = configManager2.LoadSettings();

            // Assert
            Assert.That(settings1, Is.TypeOf<AppSettings>());
            Assert.That(settings2, Is.TypeOf<AppSettingsStub>());
        }

        [Test]
        public void ShouldCreateInitialSettingsFileWhenSaving()
        {
            // Act
            _configManager.SaveSettings();

            // Assert
            Assert.That(File.Exists(_configManager.LoadSettings().GetFilePath()), Is.True);
        }

        [Test]
        public void ShouldCreateInitialSettingsFileWhenLoading()
        {
            // Act
            _ = _configManager.LoadSettings();

            // Assert
            Assert.That(File.Exists(_configManager.LoadSettings().GetFilePath()), Is.True);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ShouldSaveAndLoadSettings(bool condition)
        {
            // Arrange
            var appSettings = _configManager.LoadSettings();
            string name = TestValues.GetRandomString();
            appSettings.Condition = condition;
            appSettings.Name = name;

            // Act
            _configManager.ApplySettings(appSettings);
            _configManager.SaveSettings();
            appSettings = _configManager.LoadSettings();

            // Assert
            Assert.That(appSettings.Condition, Is.EqualTo(condition));
            Assert.That(appSettings.Name, Is.EqualTo(name));
        }

        [Test]
        public void ShouldMakeFileReadonlyOnSave()
        {
            // Act
            _configManager.SaveSettings();

            // Assert
            Assert.That(File.GetAttributes(_configManager.LoadSettings().GetFilePath()).HasFlag(FileAttributes.ReadOnly));
        }
    }
}
