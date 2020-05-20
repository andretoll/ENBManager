using ENBManager.Configuration.Interfaces;
using ENBManager.Configuration.Models;
using ENBManager.Core.Helpers;
using ENBManager.Core.Interfaces;
using ENBManager.Core.Views;
using ENBManager.Infrastructure.BusinessEntities;
using Prism.Commands;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ENBManager.Core.ViewModels
{
    public class SideMenuViewModel : BindableBase
    {
        #region Private Members

        private readonly IConfigurationManager<AppSettings> _configurationManager;
        private readonly IDialogService _dialogService;
        private readonly IFileService _fileService;
        private readonly IModuleCatalog _moduleCatalog;
        private readonly IModuleManager _moduleManager;

        private InstalledGame _selectedGame;

        #endregion

        #region Public Properties

        public ObservableCollection<InstalledGame> Games { get; set; }
        public InstalledGame SelectedGame
        {
            get { return _selectedGame; }
            set
            {
                _selectedGame = value;
                RaisePropertyChanged();

                if (value != null)
                    ActivateModule(value.Module);
            }
        }

        #endregion

        #region Helper Properties

        public bool DarkMode
        {
            get { return _configurationManager.Settings.DarkMode; }
            set
            {
                _configurationManager.Settings.DarkMode = value;
                _configurationManager.SaveSettings();

                ThemeHelper.UpdateTheme(value);
            }
        }
        public bool ShowDarkModeShortcut => _configurationManager.Settings.DarkModeShortcut;

        #endregion

        #region Commands

        public DelegateCommand GetDataCommand { get; set; }
        public DelegateCommand OpenSettingsCommand { get; set; }
        public DelegateCommand OpenDiscoverGamesCommand { get; set; }

        #endregion

        #region Constructor

        public SideMenuViewModel(
            IConfigurationManager<AppSettings> configurationManager,
            IDialogService dialogService,
            IFileService fileService, 
            IModuleCatalog moduleCatalog, 
            IModuleManager moduleManager)
        {
            _configurationManager = configurationManager;
            _dialogService = dialogService;
            _fileService = fileService;
            _moduleCatalog = moduleCatalog;
            _moduleManager = moduleManager;

            GetDataCommand = new DelegateCommand(OnGetDataCommand);
            OpenSettingsCommand = new DelegateCommand(OnOpenSettingsCommand);
            OpenDiscoverGamesCommand = new DelegateCommand(OnOpenDiscoverGamesCommand);
        }

        #endregion

        #region Private Methods

        private void OnGetDataCommand()
        {
            Games = new ObservableCollection<InstalledGame>();

            var directories = _fileService.GetGameDirectories();

            foreach (var game in directories)
            {
                var moduleInfo = _moduleCatalog.Modules.SingleOrDefault(x => x.ModuleName == new DirectoryInfo(game).Name);
                var module = (InstalledGame)InstanceFactory.CreateInstance(Type.GetType(moduleInfo.ModuleType));
                Games.Add(module);
            }

            if (_configurationManager.Settings.OpenLastActiveGame && !string.IsNullOrEmpty(_configurationManager.Settings.LastActiveGame))
                _selectedGame = Games.FirstOrDefault(x => x.Module == _configurationManager.Settings.LastActiveGame);

            RaisePropertyChanged(nameof(Games));
            RaisePropertyChanged(nameof(SelectedGame));
        }

        private void OnOpenSettingsCommand()
        {
            _dialogService.ShowDialog(nameof(AppSettingsDialog), new DialogParameters(), (dr) =>
            {
                RaisePropertyChanged(nameof(DarkMode));
                RaisePropertyChanged(nameof(ShowDarkModeShortcut));
            });
        }

        private void OnOpenDiscoverGamesCommand()
        {
            _dialogService.ShowDialog(nameof(DiscoverGamesDialog), new DialogParameters(), (dr) =>
            {
                OnGetDataCommand();
            });
        }

        private void ActivateModule(string name)
        {
            var module = _moduleCatalog.Modules.SingleOrDefault(x => x.ModuleName == name);
            _moduleManager.LoadModule(module.ModuleName);

            _configurationManager.Settings.LastActiveGame = name;
            _configurationManager.SaveSettings();
        }

        #endregion
    }
}
