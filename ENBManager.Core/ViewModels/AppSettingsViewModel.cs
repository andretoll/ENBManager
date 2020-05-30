using ENBManager.Configuration.Interfaces;
using ENBManager.Core.Helpers;
using ENBManager.Infrastructure.BusinessEntities;
using NLog;
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

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IConfigurationManager<AppSettings> _configurationManager;

        #endregion

        #region Public Properties

        public AppSettings Settings => _configurationManager.Settings;

        public IEnumerable<string> ColorSchemes => ThemeHelper.GetColorSchemes().Select(x => x.Name);

        #endregion

        #region Commands

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }

        #endregion

        #region Constructor

        public AppSettingsViewModel(IConfigurationManager<AppSettings> configurationManager)
        {
            _configurationManager = configurationManager;

            SaveCommand = new DelegateCommand(OnSaveCommand);
            CancelCommand = new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel)));

            _logger.Debug($"{nameof(AppSettingsViewModel)} initialized");
        }

        #endregion

        #region Private Methods

        private void OnSaveCommand()
        {
            _logger.Debug(nameof(OnSaveCommand));

            ThemeHelper.UpdateTheme(_configurationManager.Settings.DarkMode);
            ThemeHelper.UpdateColorScheme(_configurationManager.Settings.ColorScheme);

            _configurationManager.SaveSettings();

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        #endregion

        #region IDialogAware Implementation

        public string Title => Localization.Strings.Strings.SETTINGS;

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() 
        {
            _logger.Info(nameof(OnDialogClosed));
        }

        public void OnDialogOpened(IDialogParameters parameters) 
        {
            _logger.Info(nameof(OnDialogOpened));
        }

        #endregion
    }
}
