using ENBManager.Core.Constants;
using ENBManager.Core.Views;
using NLog;
using Prism.Mvvm;
using Prism.Regions;

namespace ENBManager.Core.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        #region Private Members

        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IRegionManager _regionManager;

        #endregion

        #region Constructor

        public ShellViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            InitializeViews();
        } 

        #endregion

        #region Private Methods

        private void InitializeViews()
        {
            _logger.Debug(nameof(InitializeViews));

            _regionManager.RegisterViewWithRegion(RegionNames.SideMenuRegion, typeof(SideMenuView));
            _regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(MainView));
        }

        #endregion
    }
}
