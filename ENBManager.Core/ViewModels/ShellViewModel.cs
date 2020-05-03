using ENBManager.Core.Constants;
using ENBManager.Core.Views.UserControls;
using Prism.Mvvm;
using Prism.Regions;
using System;

namespace ENBManager.Core.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        #region Private Members

        private readonly IRegionManager _regionManager;

        #endregion

        public ShellViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            Initialize();
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.SideMenuRegion, typeof(SideMenuView));
            _regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(MainView));
        }
    }
}
