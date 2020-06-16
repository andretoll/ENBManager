using ENBManager.Configuration.Interfaces;
using ENBManager.Configuration.Services;
using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.Helpers;
using NLog;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace ENBManager.Core.ViewModels
{
    public class GameSettingsViewModel : BindableBase, IDialogAware
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private IConfigurationManager<GameSettings> _configurationManager;

        #endregion

        #region Public Properties

        public GameSettings Settings => _configurationManager?.Settings;

        #endregion

        #region Helper Properties

        public string VirtualExecutablePath
        {
            get { return _configurationManager?.Settings.VirtualExecutablePath; }
            set
            {
                _configurationManager.Settings.VirtualExecutablePath = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Commands

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand BrowseCommand { get; }

        #endregion

        #region Constructor

        public GameSettingsViewModel()
        {
            SaveCommand = new DelegateCommand(OnSaveCommand);
            CancelCommand = new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel)));
            BrowseCommand = new DelegateCommand(OnBrowseCommand);
        }

        #endregion

        #region Private Methods

        private void OnSaveCommand()
        {
            _logger.Info("Saving app settings");

            _configurationManager.SaveSettings();
            _configurationManager.LoadSettings();

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        private void OnBrowseCommand()
        {
            _logger.Info("Browsing");

            string fileName = DialogHelper.OpenExecutable();

            if (string.IsNullOrEmpty(fileName))
                return;

            VirtualExecutablePath = fileName;
        }

        #endregion

        #region IDialogAware Implementation

        public string Title { get; set; }

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() 
        {
            _logger.Info("Closed");
        }

        public void OnDialogOpened(IDialogParameters parameters) 
        {
            _logger.Info("Opened");

            if (parameters.GetValue<string>("Title") != null)
                Title = parameters.GetValue<string>("Title");

            if (parameters.GetValue<GameSettings>("GameSettings") != null)
                _configurationManager = new ConfigurationManager<GameSettings>(parameters.GetValue<GameSettings>("GameSettings"));

            RaisePropertyChanged(nameof(Settings));
        }

        #endregion
    }
}
