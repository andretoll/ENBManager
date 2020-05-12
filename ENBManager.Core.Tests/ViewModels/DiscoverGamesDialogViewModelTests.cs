using ENBManager.Core.Tests.Stubs;
using ENBManager.Core.ViewModels;
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
            var fileService = new FileServiceStub();
            var gameRegistry = new GameRegistryStub();
            var gameLocator = new GameLocatorStub();
            _viewModel = new DiscoverGamesDialogViewModel(fileService, gameLocator, gameRegistry);
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
            Assert.That(_viewModel.Games.Where(x => x.Installed).Count(), Is.GreaterThan(0));
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
    }
}
