using ENBManager.Configuration.Interfaces;
using ENBManager.Configuration.Services;
using ENBManager.Core.Helpers;
using ENBManager.Core.Interfaces;
using ENBManager.Core.Views;
using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Modules.Shared.Interfaces;
using NLog;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ENBManager.Core.ViewModels
{
    public class SideMenuViewModel : BindableBase
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IConfigurationManager<AppSettings> _configurationManager;
        private readonly IDialogService _dialogService;
        private readonly IFileService _fileService;
        private readonly IGameModuleCatalog _gameModuleCatalog;
        private readonly IRegionManager _regionManager;

        private GameModule _selectedGame;

        #endregion

        #region Public Properties

        public ObservableCollection<GameModule> Games { get; set; }
        public GameModule SelectedGame
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

        public DelegateCommand GetDataCommand { get; }
        public DelegateCommand OpenSettingsCommand { get; }
        public DelegateCommand OpenDiscoverGamesCommand { get; }

        #endregion

        #region Constructor

        public SideMenuViewModel(
            IConfigurationManager<AppSettings> configurationManager,
            IDialogService dialogService,
            IFileService fileService, 
            IGameModuleCatalog gameModuleCatalog, 
            IRegionManager regionManager)
        {
            _configurationManager = configurationManager;
            _dialogService = dialogService;
            _fileService = fileService;
            _gameModuleCatalog = gameModuleCatalog;
            _regionManager = regionManager;

            GetDataCommand = new DelegateCommand(OnGetDataCommand);
            OpenSettingsCommand = new DelegateCommand(OnOpenSettingsCommand);
            OpenDiscoverGamesCommand = new DelegateCommand(OnOpenDiscoverGamesCommand);

            _logger.Debug($"{nameof(SideMenuViewModel)} initialized");
        }

        #endregion

        #region Private Methods

        private void OnGetDataCommand()
        {
            _logger.Debug(nameof(OnGetDataCommand));

            Games = new ObservableCollection<GameModule>();

            var directories = _fileService.GetGameDirectories();

            foreach (var directory in directories)
            {
                var game = _gameModuleCatalog.GameModules.Single(x => x.Module == Path.GetFileName(directory));

                game.Settings = 
                    ConfigurationManager<GameSettings>.LoadSettings(
                        new GameSettings(game.Module).GetFullPath());

                Games.Add(game);
            }

            if (_configurationManager.Settings.OpenLastActiveGame && !string.IsNullOrEmpty(_configurationManager.Settings.LastActiveGame))
                SelectedGame = Games.FirstOrDefault(x => x.Module == _configurationManager.Settings.LastActiveGame);

            RaisePropertyChanged(nameof(Games));
            RaisePropertyChanged(nameof(SelectedGame));

            _logger.Info("Game list initialized");
        }

        private void OnOpenSettingsCommand()
        {
            _logger.Debug(nameof(OnOpenSettingsCommand));

            _dialogService.ShowDialog(nameof(AppSettingsDialog), new DialogParameters(), (dr) =>
            {
                if (dr.Result == ButtonResult.OK)
                {
                    _configurationManager.LoadSettings();

                    RaisePropertyChanged(nameof(DarkMode));
                    RaisePropertyChanged(nameof(ShowDarkModeShortcut));
                }
            });
        }

        private void OnOpenDiscoverGamesCommand()
        {
            _logger.Debug(nameof(OnOpenDiscoverGamesCommand));

            DialogParameters dp = new DialogParameters
            {
                { "Games", Games }
            };

            _dialogService.ShowDialog(nameof(DiscoverGamesDialog), dp, (dr) =>
            {
                OnGetDataCommand();
            });
        }

        private void ActivateModule(string name)
        {
            var gameModule = Games.Single(x => x.Module == name);
            gameModule.Activate();

            _configurationManager.Settings.LastActiveGame = name;
            _configurationManager.SaveSettings();

            _logger.Info($"Module '{name}' loaded");
        }

        #endregion
    }
}
