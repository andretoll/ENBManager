using ENBManager.Core.Tests.Stubs;
using ENBManager.Core.ViewModels;
using ENBManager.Modules.Fallout4;
using ENBManager.Modules.Skyrim;
using ENBManager.Modules.SkyrimSE;
using NUnit.Framework;
using Prism.Modularity;
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
            var fileService = new FileServiceStub();
            var gameLocator = new GameLocatorStub();
            var moduleCatalog = GetModuleCatalog();
            _viewModel = new DiscoverGamesDialogViewModel(fileService, gameLocator, moduleCatalog);
        }

        [TearDown]
        public void Teardown()
        {

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

        private IModuleCatalog GetModuleCatalog()
        {
            ModuleCatalog moduleCatalog = new ModuleCatalog();

            moduleCatalog.AddModule(SkyrimModule.GetModuleInfo());
            moduleCatalog.AddModule(SkyrimSEModule.GetModuleInfo());
            moduleCatalog.AddModule(Fallout4Module.GetModuleInfo());

            return moduleCatalog;
        }
    }
}
