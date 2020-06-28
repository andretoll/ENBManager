using ENBManager.Core.ViewModels;
using NUnit.Framework;

namespace ENBManager.Core.Tests.ViewModels
{
    [TestFixture]
    public class AboutViewModelTests
    {
        private AboutViewModel _viewModel;

        [SetUp]
        public void Setup()
        {
            _viewModel = new AboutViewModel();
        }

        [Test]
        public void ShouldHaveDialogTitle()
        {
            // Assert
            Assert.That(string.IsNullOrEmpty(_viewModel.Title), Is.False);
        }

        [Test]
        public void ShouldHaveAppName()
        {
            // Assert
            Assert.That(string.IsNullOrEmpty(_viewModel.Name), Is.False);
        }

        [Test]
        public void ShouldHaveAppVersion()
        {
            // Assert
            Assert.That(string.IsNullOrEmpty(_viewModel.Version), Is.False);
        }
    }
}
