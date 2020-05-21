using ENBManager.Core.Helpers;
using ENBManager.Core.Interfaces;
using ENBManager.Infrastructure.BusinessEntities;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ENBManager.Core.ViewModels
{
    public class AppSettingsViewModel : BindableBase, IDialogAware
    {
        #region Private Members

        private readonly IConfigurationManager<AppSettings> _configurationManager;

        #endregion

        #region Public Properties

        public AppSettings Settings => _configurationManager.Settings;

        public IEnumerable<string> ColorSchemes { get; set; } = ThemeHelper.GetColorSchemes().Select(x => x.Name);

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
            UpdateColorScheme();

            _configurationManager.SaveSettings();

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        private void UpdateTheme()
        {
            ThemeHelper.UpdateTheme(_configurationManager.Settings.DarkMode);
        }

        private void UpdateColorScheme()
        {
            ThemeHelper.UpdateColorScheme(_configurationManager.Settings.ColorScheme);
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
