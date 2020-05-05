using ENBManager.Configuration.Interfaces;
using ENBManager.Core.Constants;
using ENBManager.Core.Views;
using ENBManager.Infrastructure.BusinessEntities;
using Prism.Mvvm;
using Prism.Regions;

namespace ENBManager.Core.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        #region Private Members

        private readonly IRegionManager _regionManager;
        private readonly IConfigurationManager<AppSettings> _configManager;

        #endregion

        public ShellViewModel(IRegionManager regionManager, IConfigurationManager<AppSettings> configManager)
        {
            _regionManager = regionManager;
            _configManager = configManager;

            InitializeViews();
        }

        #region Private Methods

        private void InitializeViews()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.SideMenuRegion, typeof(SideMenuView));
            _regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(MainView));
        }

        #endregion
    }
}
