using ENBManager.Configuration.Interfaces;
using ENBManager.Core.Views;
using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.Constants;
using ENBManager.Modules.Shared.Views;
using NLog;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;

namespace ENBManager.Core.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IDialogService _dialogService;
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
        public DelegateCommand OpenSettingsCommand { get; set; }

        #endregion

        #region Constructor

        public ShellViewModel(
            IConfigurationManager<AppSettings> configurationManager,
            IDialogService dialogService,
            IRegionManager regionManager)
        {
            _dialogService = dialogService;
            _regionManager = regionManager;

            MinimizeToTray = configurationManager.Settings.MinimizeToTray;

            RestoreApplicationCommand = new DelegateCommand(OnRestoreApplicationCommand);
            ExitApplicationCommand = new DelegateCommand(OnExitApplicationCommand);
            OpenSettingsCommand = new DelegateCommand(OnOpenSettingsCommand);

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

        private void OnOpenSettingsCommand()
        {
            _dialogService.ShowDialog(nameof(AppSettingsDialog), new DialogParameters(), null);
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
