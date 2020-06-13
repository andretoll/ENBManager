using ENBManager.Configuration.Interfaces;
using ENBManager.Core.Views;
using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.Constants;
using ENBManager.Modules.Shared.Views;
using NLog;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;

namespace ENBManager.Core.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IRegionManager _regionManager;

        private bool _showNotifyIcon;

        #endregion

        #region Public Properties

        public bool ShowNotifyIcon
        {
            get { return _showNotifyIcon; }
            set
            {
                _showNotifyIcon = value;
                RaisePropertyChanged();
            }
        }

        public bool MinimizeToTray { get; set; }

        #endregion

        #region Commands

        public DelegateCommand RestoreApplicationCommand { get; set; }
        public DelegateCommand ExitApplicationCommand { get; set; }

        #endregion

        #region Constructor

        public ShellViewModel(
            IConfigurationManager<AppSettings> configurationManager,
            IRegionManager regionManager)
        {
            _regionManager = regionManager;

            MinimizeToTray = configurationManager.Settings.MinimizeToTray;

            RestoreApplicationCommand = new DelegateCommand(OnRestoreApplicationCommand);
            ExitApplicationCommand = new DelegateCommand(OnExitApplicationCommand);

            InitializeViews();
        }

        #endregion

        #region Events

        public event EventHandler ExitApplicationEventHandler;
        public event EventHandler RestoreApplicationEventHandler;

        #endregion

        #region Private Methods

        private void OnRestoreApplicationCommand()
        {
            RestoreApplicationEventHandler.Invoke(null, null);
        }

        private void OnExitApplicationCommand()
        {
            ExitApplicationEventHandler.Invoke(null, null);
        }

        private void InitializeViews()
        {
            _logger.Debug("Initializing views");

            _regionManager.RegisterViewWithRegion(RegionNames.SideMenuRegion, typeof(SideMenuView));
            _regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(MainView));
            _regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(ModuleShell));
        }

        #endregion
    }
}
