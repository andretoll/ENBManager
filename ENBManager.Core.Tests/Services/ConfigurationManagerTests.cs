using ENBManager.Core.Interfaces;
using ENBManager.Core.Services;
using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.TestUtils.Utils;
using NUnit.Framework;
using System.IO;

namespace ENBManager.Core.Tests.Services
{
    [TestFixture]
    public class ConfigurationManagerTests
    {
        private IConfigurationManager<AppSettingsStub> _appConfig;
        private IConfigurationManager<GameSettingsStub> _gameConfig;

        [SetUp]
        public void Setup()
        {
            _appConfig = new ConfigurationManager<AppSettingsStub>();
            _gameConfig = new ConfigurationManager<GameSettingsStub>(new GameSettingsStub());
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(Path.Combine(_appConfig.Settings.GetFilePath())))
            {
                File.SetAttributes(Path.Combine(_appConfig.Settings.GetFilePath()), FileAttributes.Normal);
                Directory.Delete(Path.GetDirectoryName(_appConfig.Settings.GetFilePath()), true);
            }

            if (File.Exists(Path.Combine(_gameConfig.Settings.GetFilePath())))
            {
                File.SetAttributes(Path.Combine(_gameConfig.Settings.GetFilePath()), FileAttributes.Normal);
                Directory.Delete(Path.GetDirectoryName(_gameConfig.Settings.GetFilePath()), true);
            }
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
            _appConfig.SaveSettings();

            // Assert
            Assert.That(File.Exists(_appConfig.Settings.GetFilePath()), Is.True);
        }

        [Test]
        public void ShouldCreateInitialSettingsFileWhenLoading()
        {
            // Act
            _appConfig.LoadSettings();

            // Assert
            Assert.That(File.Exists(_appConfig.Settings.GetFilePath()), Is.True);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ShouldSaveAndLoadSettings(bool condition)
        {
            // Arrange
            var appSettings = _appConfig.Settings;
            string name = TestValues.GetRandomString();
            appSettings.Condition = condition;
            appSettings.Name = name;

            // Act
            _appConfig.SaveSettings();
            _appConfig.LoadSettings();
            appSettings = _appConfig.Settings;

            // Assert
            Assert.That(appSettings.Condition, Is.EqualTo(condition));
            Assert.That(appSettings.Name, Is.EqualTo(name));
        }

        [Test]
        public void ShouldMakeFileReadonlyOnSave()
        {
            // Act
            _appConfig.SaveSettings();

            // Assert
            Assert.That(File.GetAttributes(_appConfig.Settings.GetFilePath()).HasFlag(FileAttributes.ReadOnly));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ShouldLoadSettingsOnConstruct(bool condition)
        {
            // Arrange
            string random = TestValues.GetRandomString();
            _appConfig.Settings.Condition = condition;
            _appConfig.Settings.Name = random;
            _appConfig.SaveSettings();

            // Act
            _appConfig = new ConfigurationManager<AppSettingsStub>();

            // Assert
            Assert.That(_appConfig.Settings.Condition, Is.EqualTo(condition));
            Assert.That(_appConfig.Settings.Name, Is.EqualTo(random));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ShouldInitializeOnlyIfFileDoesNotExist(bool condition)
        {
            // Arrange
            string wrongValue = TestValues.GetRandomString();
            string name = TestValues.GetRandomString();
            _gameConfig.Settings.Name = name;
            _gameConfig.Settings.Condition = condition;
            _gameConfig.Initialize();

            // Act
            _gameConfig.LoadSettings();
            _gameConfig.Settings.Name = wrongValue;
            _gameConfig.Initialize();
            _gameConfig.LoadSettings();

            // Assert
            Assert.That(_gameConfig.Settings.Name, Is.EqualTo(name));
            Assert.That(_gameConfig.Settings.Condition, Is.EqualTo(condition));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ShouldLoadSettingsWithoutInitializing(bool condition)
        {
            // Arrange
            var appSettings = _appConfig.Settings;
            string name = TestValues.GetRandomString();
            appSettings.Condition = condition;
            appSettings.Name = name;

            // Act
            _appConfig.SaveSettings();
            var newAppSettings = ConfigurationManager<AppSettingsStub>.LoadSettings(appSettings.GetFilePath());

            // Assert
            Assert.That(newAppSettings.Condition, Is.EqualTo(condition));
            Assert.That(newAppSettings.Name, Is.EqualTo(name));
        }
    }
}
