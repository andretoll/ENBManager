using ENBManager.Configuration.Interfaces;
using ENBManager.Configuration.Services;
using ENBManager.Configuration.Tests.Stubs;
using ENBManager.TestUtils.Utils;
using NUnit.Framework;
using System.IO;

namespace ENBManager.Configuration.Tests.Services
{
    [TestFixture]
    public class ConfigurationManagerTests
    {
        private IConfigurationManager<SettingsAStub> _settingsAConfig;
        private IConfigurationManager<SettingsBStub> _settingsBConfig;

        [SetUp]
        public void Setup()
        {
            TearDown();

            _settingsAConfig = new ConfigurationManager<SettingsAStub>();
        }

        [TearDown]
        public void TearDown()
        {
            _settingsAConfig = new ConfigurationManager<SettingsAStub>();
            _settingsBConfig = new ConfigurationManager<SettingsBStub>();

            if (File.Exists(_settingsAConfig.Settings.GetFullPath()))
            {
                File.SetAttributes(Path.Combine(_settingsAConfig.Settings.GetFullPath()), FileAttributes.Normal);
                Directory.Delete(Path.GetDirectoryName(_settingsAConfig.Settings.GetFullPath()), true);
            }

            if (File.Exists(_settingsBConfig.Settings.GetFullPath()))
            {
                File.SetAttributes(Path.Combine(_settingsBConfig.Settings.GetFullPath()), FileAttributes.Normal);
                Directory.Delete(Path.GetDirectoryName(_settingsBConfig.Settings.GetFullPath()), true);
            }
        }

        [Test]
        public void ShouldCreateSettingsInstanceOfAnyDerivedType()
        {
            // Arrange
            _settingsAConfig = new ConfigurationManager<SettingsAStub>();
            _settingsBConfig = new ConfigurationManager<SettingsBStub>();

            // Act
            var settingsA = _settingsAConfig.Settings;
            var settingsB = _settingsBConfig.Settings;

            // Assert
            Assert.That(settingsA.GetType(), Is.EqualTo(typeof(SettingsAStub)));
            Assert.That(settingsB.GetType(), Is.EqualTo(typeof(SettingsBStub)));
        }

        [Test]
        public void ShouldCreateFileWhenSavingIfNotExisting()
        {
            // Arrange
            _settingsAConfig = new ConfigurationManager<SettingsAStub>();

            // Act
            _settingsAConfig.SaveSettings();

            // Assert
            Assert.That(File.Exists(_settingsAConfig.Settings.GetFullPath()));
        }

        [Test]
        public void ShouldCreateFileWhenLoadingIfNotExisting()
        {
            // Arrange
            _settingsAConfig = new ConfigurationManager<SettingsAStub>();

            // Act
            _settingsAConfig.LoadSettings();

            // Assert
            Assert.That(File.Exists(_settingsAConfig.Settings.GetFullPath()));
        }

        [Test]
        public void ShouldMakeFileReadonlyOnSave()
        {
            // Act
            _settingsAConfig.SaveSettings();

            // Assert
            Assert.That(File.GetAttributes(_settingsAConfig.Settings.GetFullPath()).HasFlag(FileAttributes.ReadOnly));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ShouldSaveAndLoadSettingsCorrectly(bool condition)
        {
            // Arrange
            var appSettings = _settingsAConfig.Settings;
            string text = TestValues.GetRandomString();
            appSettings.Condition = condition;
            appSettings.Text = text;

            // Act
            _settingsAConfig.SaveSettings();
            _settingsAConfig.LoadSettings();
            appSettings = _settingsAConfig.Settings;

            // Assert
            Assert.That(appSettings.Condition, Is.EqualTo(condition));
            Assert.That(appSettings.Text, Is.EqualTo(text));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ShouldLoadSettingsOnConstruct(bool condition)
        {
            // Arrange
            string text = TestValues.GetRandomString();
            _settingsAConfig.Settings.Condition = condition;
            _settingsAConfig.Settings.Text = text;
            _settingsAConfig.SaveSettings();

            // Act
            _settingsAConfig = new ConfigurationManager<SettingsAStub>();

            // Assert
            Assert.That(_settingsAConfig.Settings.Condition, Is.EqualTo(condition));
            Assert.That(_settingsAConfig.Settings.Text, Is.EqualTo(text));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ShouldLoadSettingsWithoutInitializing(bool condition)
        {
            // Arrange
            var appSettings = _settingsAConfig.Settings;
            string text = TestValues.GetRandomString();
            appSettings.Condition = condition;
            appSettings.Text = text;

            // Act
            _settingsAConfig.SaveSettings();
            var newAppSettings = ConfigurationManager<SettingsAStub>.LoadSettings(appSettings.GetFullPath());

            // Assert
            Assert.That(newAppSettings.Condition, Is.EqualTo(condition));
            Assert.That(newAppSettings.Text, Is.EqualTo(text));
        }
    }
}
