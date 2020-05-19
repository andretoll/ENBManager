using ENBManager.Configuration.Interfaces;
using ENBManager.Configuration.Models;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace ENBManager.Core.ViewModels
{
    public class AppSettingsViewModel : BindableBase, IDialogAware
    {
        #region Constructor

        public AppSettingsViewModel(IConfigurationManager<AppSettings> configurationManager)
        {

        }

        #endregion

        #region IDialogAware Implementation

        public string Title => Localization.Strings.Strings.SETTINGS;

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { }

        public void OnDialogOpened(IDialogParameters parameters) { }

        #endregion
    }
}
