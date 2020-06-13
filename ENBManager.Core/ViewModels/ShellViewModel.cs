using ENBManager.Configuration.Interfaces;
using ENBManager.Core.Views;
using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.Constants;
using ENBManager.Modules.Shared.Events;
using ENBManager.Modules.Shared.Views;
using NLog;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Reflection;

namespace ENBManager.Core.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;

        private bool _showNotifyIcon;
        private bool _enableScreenshots;

        #endregion

        #region Public Properties

        public string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();

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

        public bool EnableScreenshots
        {
            get { return _enableScreenshots; }
            set
            {
                _enableScreenshots = value;

                _eventAggregator.GetEvent<ScreenshotsStatusChangedExternalEvent>().Publish(value);
            }
        }

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
            IEventAggregator eventAggregator,
            IRegionManager regionManager)
        {
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;

            MinimizeToTray = configurationManager.Settings.MinimizeToTray;

            eventAggregator.GetEvent<ScreenshotsStatusChangedModuleEvent>().Subscribe(OnScreenshotStatusChanged);

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

        private void OnScreenshotStatusChanged(bool enabled)
        {
            _enableScreenshots = enabled;
            RaisePropertyChanged(nameof(EnableScreenshots));
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
