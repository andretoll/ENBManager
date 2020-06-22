using ENBManager.Core.ViewModels;
using ENBManager.Infrastructure.BusinessEntities;
using NUnit.Framework;
using Prism.Services.Dialogs;

namespace ENBManager.Core.Tests.ViewModels
{
    [TestFixture]
    public class GameSettingsViewModelTests
    {
        private GameSettingsViewModel _viewModel;

        [SetUp]
        public void Setup()
        {
            _viewModel = new GameSettingsViewModel();
        }

        [TestCase("Skyrim")]
        [TestCase("Fallout 4")]
        [TestCase("Fallout 3")]
        public void ShouldHaveDialogTitle(string title)
        {
            // Arrange
            DialogParameters dp = new DialogParameters()
            {
                { "Title", title }
            };

            // Act
            _viewModel.OnDialogOpened(dp);

            // Assert
            Assert.That(string.IsNullOrEmpty(_viewModel.Title), Is.False);
            Assert.That(_viewModel.Title, Is.EqualTo(title));
        }

        [TestCase(true, "C:\\Dir\\Subdir\\Subsubdir")]
        [TestCase(false, "C:\\Dir\\Subdir\\Subsubdir")]
        [TestCase(true, null)]
        [TestCase(false, null)]
        public void ShouldInitializeSettings(bool enabled, string path)
        {
            // Arrange
            var gameSettings = new GameSettings();
            gameSettings.VirtualExecutableEnabled = enabled;
            gameSettings.VirtualExecutablePath = path;
            DialogParameters dp = new DialogParameters()
            {
                { "GameSettings", gameSettings }
            };

            // Act
            _viewModel.OnDialogOpened(dp);

            // Assert
            Assert.That(_viewModel.Settings.VirtualExecutableEnabled, Is.EqualTo(enabled));
            Assert.That(_viewModel.Settings.VirtualExecutablePath, Is.EqualTo(path));
            Assert.That(_viewModel.VirtualExecutablePath, Is.EqualTo(path));
        }
    }
}
