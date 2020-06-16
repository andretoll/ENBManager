using ENBManager.Configuration.Interfaces;
using ENBManager.Core.Helpers;
using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.Helpers;
using NLog;
using NLog.Targets;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
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
        public DelegateCommand ExportLogFileCommand { get; set; }

        #endregion

        #region Constructor

        public AppSettingsViewModel(IConfigurationManager<AppSettings> configurationManager)
        {
            _configurationManager = configurationManager;

            SaveCommand = new DelegateCommand(OnSaveCommand);
            CancelCommand = new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel)));
            ExportLogFileCommand = new DelegateCommand(OnExportLogFileCommand);
        }

        #endregion

        #region Private Methods

        private void OnSaveCommand()
        {
            _logger.Info("Saving app settings");

            ThemeHelper.UpdateTheme(_configurationManager.Settings.DarkMode);
            ThemeHelper.UpdateColorScheme(_configurationManager.Settings.ColorScheme);

            _configurationManager.SaveSettings();
            _configurationManager.LoadSettings();

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        private void OnExportLogFileCommand()
        {
            _logger.Info("Exporting log file");

            string logFilePath = (LogManager.Configuration.AllTargets[0] as FileTarget).FileName.Render(new LogEventInfo());
            DialogHelper.SaveFile(logFilePath, Path.GetFileName(logFilePath), "Log files (*.log)|*.log");
        }

        #endregion

        #region IDialogAware Implementation

        public string Title => Localization.Strings.Strings.SETTINGS;

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() 
        {
            _logger.Info("Closed");
        }

        public void OnDialogOpened(IDialogParameters parameters) 
        {
            _logger.Info("Opened");
        }

        #endregion
    }
}
