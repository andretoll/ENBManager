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
            File.SetAttributes(Path.Combine(_configManager.Settings.GetFilePath()), FileAttributes.Normal);
            Directory.Delete(Path.GetDirectoryName(_configManager.Settings.GetFilePath()), true);
        }

        [Test]
        public void ShouldInitializeWithAnyDerivedClass()
        {
            // Arrange
            var configManager1 = new ConfigurationManager<AppSettings>();
            var configManager2 = new ConfigurationManager<AppSettingsStub>();

            // Act
            var settings1 = configManager1.Settings;
            var settings2 = configManager2.Settings;

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
            Assert.That(File.Exists(_configManager.Settings.GetFilePath()), Is.True);
        }

        [Test]
        public void ShouldCreateInitialSettingsFileWhenLoading()
        {
            // Act
            _configManager.LoadSettings();

            // Assert
            Assert.That(File.Exists(_configManager.Settings.GetFilePath()), Is.True);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ShouldSaveAndLoadSettings(bool condition)
        {
            // Arrange
            var appSettings = _configManager.Settings;
            string name = TestValues.GetRandomString();
            appSettings.Condition = condition;
            appSettings.Name = name;

            // Act
            _configManager.SaveSettings();
            appSettings = _configManager.Settings;

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
            Assert.That(File.GetAttributes(_configManager.Settings.GetFilePath()).HasFlag(FileAttributes.ReadOnly));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ShouldLoadSettingsOnConstruct(bool condition)
        {
            // Arrange
            string random = TestValues.GetRandomString();
            _configManager.Settings.Condition = condition;
            _configManager.Settings.Name = random;
            _configManager.SaveSettings();

            // Act
            _configManager = new ConfigurationManager<AppSettingsStub>();

            // Assert
            Assert.That(_configManager.Settings.Condition, Is.EqualTo(condition));
            Assert.That(_configManager.Settings.Name, Is.EqualTo(random));
        }
    }
}
