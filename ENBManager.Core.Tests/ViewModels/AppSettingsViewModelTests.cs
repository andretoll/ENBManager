using ENBManager.Configuration.Interfaces;
using ENBManager.Configuration.Services;
using ENBManager.Core.ViewModels;
using ENBManager.Infrastructure.BusinessEntities;
using NUnit.Framework;

namespace ENBManager.Core.Tests.ViewModels
{
    [TestFixture]
    public class AppSettingsViewModelTests
    {
        private AppSettingsViewModel _viewModel;

        [SetUp]
        public void Setup()
        {
            IConfigurationManager<AppSettings> configManager = new ConfigurationManager<AppSettings>();
            _viewModel = new AppSettingsViewModel(configManager);
        }

        [Test]
        public void ShouldHaveDialogTitle()
        {
            // Assert
            Assert.That(string.IsNullOrEmpty(_viewModel.Title), Is.False);
        }
    }
}
