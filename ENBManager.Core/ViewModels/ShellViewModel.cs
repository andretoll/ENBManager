using ENBManager.Configuration.Interfaces;
using ENBManager.Configuration.Models;
using ENBManager.Core.Constants;
using ENBManager.Core.Views;
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

        #region Constructor

        public ShellViewModel(IConfigurationManager<AppSettings> configManager, IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _configManager = configManager;

            InitializeViews();
        } 

        #endregion

        #region Private Methods

        private void InitializeViews()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.SideMenuRegion, typeof(SideMenuView));
            _regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(MainView));
        }

        #endregion
    }
}
