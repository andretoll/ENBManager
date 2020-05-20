using ENBManager.Configuration.Interfaces;
using ENBManager.Configuration.Models;
using ENBManager.Core.Helpers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace ENBManager.Core.ViewModels
{
    public class AppSettingsViewModel : BindableBase, IDialogAware
    {
        #region Private Members

        private readonly IConfigurationManager<AppSettings> _configurationManager;

        #endregion

        #region Public Properties

        public AppSettings Settings => _configurationManager.Settings;

        #endregion

        #region Commands

        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        #endregion

        #region Constructor

        public AppSettingsViewModel(IConfigurationManager<AppSettings> configurationManager)
        {
            _configurationManager = configurationManager;

            SaveCommand = new DelegateCommand(OnSaveCommand);
            CancelCommand = new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel)));
        }

        #endregion

        #region Private Methods

        private void OnSaveCommand()
        {
            UpdateTheme();

            _configurationManager.SaveSettings();

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        private void UpdateTheme()
        {
            ThemeHelper.UpdateTheme(_configurationManager.Settings.DarkMode);
        }

        #endregion

        #region IDialogAware Implementation

        public string Title => Localization.Strings.Strings.SETTINGS;

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() 
        {
            _configurationManager.LoadSettings();
        }

        public void OnDialogOpened(IDialogParameters parameters) { }

        #endregion
    }
}
