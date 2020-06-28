using ENBManager.Core.Tests.Stubs;
using ENBManager.Core.ViewModels;
using ENBManager.Modules.Fallout4;
using ENBManager.Modules.Shared.Interfaces;
using ENBManager.Modules.Shared.Services;
using ENBManager.Modules.Skyrim;
using ENBManager.Modules.SkyrimSE;
using NUnit.Framework;
using System.Linq;

namespace ENBManager.Core.Tests.ViewModels
{
    [TestFixture]
    public class DiscoverGamesDialogViewModelTests
    {
        private DiscoverGamesDialogViewModel _viewModel;

        [SetUp]
        public void Setup()
        {
            var fileService = new GameServiceStub();
            var gameModuleCatalog = GetGameModuleCatalog();
            _viewModel = new DiscoverGamesDialogViewModel(fileService, gameModuleCatalog);
        }

        [Test]
        public void ShouldHaveDialogTitle()
        {
            // Assert
            Assert.That(string.IsNullOrEmpty(_viewModel.Title), Is.False);
        }

        [Test]
        public void ShouldGetData()
        {
            // Act
            _viewModel.GetDataCommand.Execute();

            // Assert
            Assert.That(_viewModel.Games, Has.Count.GreaterThan(0));
        }

        [Test]
        public void ShouldInitializeGamesAsManaged()
        {
            // Act
            _viewModel.GetDataCommand.Execute();

            // Assert
            Assert.That(_viewModel.Games.Where(x => x.Installed).All(x => x.ShouldManage));
        }

        [Test]
        public void ShouldEnableContinueWhenManagedGamesAreFound()
        {
            // Arrange
            bool expectingFalse = _viewModel.ContinueCommand.CanExecute();

            // Act
            _viewModel.GetDataCommand.Execute();
            _viewModel.Games[0].InstalledLocation = "C:\\Game";
            bool expectingTrue = _viewModel.ContinueCommand.CanExecute();
            foreach (var game in _viewModel.Games)
            {
                game.ShouldManage = false;
            }
            bool expectingFalseAgain = _viewModel.ContinueCommand.CanExecute();
            _viewModel.Games.First(x => x.Installed).ShouldManage = true;
            bool expectingTrueAgain = _viewModel.ContinueCommand.CanExecute();

            // Assert
            Assert.That(expectingFalse, Is.False);
            Assert.That(expectingTrue, Is.True);
            Assert.That(expectingFalseAgain, Is.False);
            Assert.That(expectingTrueAgain, Is.True);
        }

        private IGameModuleCatalog GetGameModuleCatalog()
        {
            GameModuleCatalog gameModuleCatalog = new GameModuleCatalog();

            gameModuleCatalog.Register<SkyrimModule>(null);
            gameModuleCatalog.Register<SkyrimSEModule>(null);
            gameModuleCatalog.Register<Fallout4Module>(null);
            gameModuleCatalog.Register<Fallout3Module>(null);
            gameModuleCatalog.Register<FalloutNVModule>(null);

            return gameModuleCatalog;
        }
    }
}
