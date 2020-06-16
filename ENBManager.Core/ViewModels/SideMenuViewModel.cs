using ENBManager.Configuration.Interfaces;
using ENBManager.Configuration.Services;
using ENBManager.Core.Helpers;
using ENBManager.Core.Views;
using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.BusinessEntities.Dialogs;
using ENBManager.Infrastructure.Constants;
using ENBManager.Infrastructure.Interfaces;
using ENBManager.Localization.Strings;
using ENBManager.Modules.Shared.Interfaces;
using NLog;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ENBManager.Core.ViewModels
{
    public class SideMenuViewModel : BindableBase
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IConfigurationManager<AppSettings> _configurationManager;
        private readonly IDialogService _dialogService;
        private readonly IGameService _gameService;
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
                else
                    _regionManager.RequestNavigate(RegionNames.MainRegion, nameof(MainView));
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

        public bool ShowRunGameShortcut => _configurationManager.Settings.RunGameShortcut;

        public bool ShowDarkModeShortcut => _configurationManager.Settings.DarkModeShortcut;

        #endregion

        #region Commands

        public DelegateCommand GetDataCommand { get; }
        public DelegateCommand OpenSettingsCommand { get; }
        public DelegateCommand OpenDiscoverGamesCommand { get; }
        public DelegateCommand OpenAboutCommand { get; set; }
        public DelegateCommand<GameModule> OpenGameDirectoryCommand { get; }
        public DelegateCommand<GameModule> OpenNexusCommand { get; }
        public DelegateCommand<GameModule> OpenScreenshotsDirectoryCommand { get; }
        public DelegateCommand<GameModule> OpenGameSettingsCommand { get; }
        public DelegateCommand<GameModule> RunCommand { get; set; }

        #endregion

        #region Constructor

        public SideMenuViewModel(
            IConfigurationManager<AppSettings> configurationManager,
            IDialogService dialogService,
            IGameService gameService, 
            IGameModuleCatalog gameModuleCatalog, 
            IRegionManager regionManager)
        {
            _configurationManager = configurationManager;
            _dialogService = dialogService;
            _gameService = gameService;
            _gameModuleCatalog = gameModuleCatalog;
            _regionManager = regionManager;

            GetDataCommand = new DelegateCommand(OnGetDataCommand);
            OpenSettingsCommand = new DelegateCommand(OnOpenSettingsCommand);
            OpenDiscoverGamesCommand = new DelegateCommand(OnOpenDiscoverGamesCommand);
            OpenAboutCommand = new DelegateCommand(OnAboutCommand);
            OpenGameDirectoryCommand = new DelegateCommand<GameModule>(OnOpenGameDirectoryCommand);
            OpenNexusCommand = new DelegateCommand<GameModule>(OnOpenNexusCommand);
            OpenScreenshotsDirectoryCommand = new DelegateCommand<GameModule>(OnOpenScreenshotsDirectoryCommand);
            OpenGameSettingsCommand = new DelegateCommand<GameModule>(OnOpenGameSettingsCommand);
            RunCommand = new DelegateCommand<GameModule>(async (x) => await OnRunCommand(x));
        }

        #endregion

        #region Private Methods

        private void OnGetDataCommand()
        {
            _logger.Info("Getting managed games");

            Games = new ObservableCollection<GameModule>();

            var directories = _gameService.GetGameDirectories();

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
            _logger.Info("Opening app settings dialog");

            _dialogService.ShowDialog(nameof(AppSettingsDialog), new DialogParameters(), (dr) =>
            {
                if (dr.Result == ButtonResult.OK)
                {
                    _configurationManager.LoadSettings();

                    RaisePropertyChanged(nameof(DarkMode));
                    RaisePropertyChanged(nameof(ShowDarkModeShortcut));
                    RaisePropertyChanged(nameof(ShowRunGameShortcut));
                }
            });
        }

        private void OnOpenDiscoverGamesCommand()
        {
            _logger.Info("Opening discover games dialog");

            DialogParameters dp = new DialogParameters
            {
                { "Games", Games }
            };

            _dialogService.ShowDialog(nameof(DiscoverGamesDialog), dp, (dr) =>
            {
                if (dr.Result == ButtonResult.OK)
                {
                    OnGetDataCommand();
                    SelectedGame = null;
                }
            });
        }

        private void OnAboutCommand()
        {
            _logger.Info("Opening about dialog");

            _dialogService.ShowDialog(nameof(AboutDialog), new DialogParameters(), null);
        }

        private void OnOpenGameDirectoryCommand(GameModule gameModule)
        {
            _logger.Info("Opening game directory");

            Process.Start("explorer", gameModule.InstalledLocation);
        }

        private void OnOpenNexusCommand(GameModule gameModule)
        {
            _logger.Info($"Opening {gameModule.Url}");

            var psi = new ProcessStartInfo
            {
                FileName = gameModule.Url,
                UseShellExecute = true
            };

            Process.Start(psi);
        }

        private void OnOpenScreenshotsDirectoryCommand(GameModule gameModule)
        {
            _logger.Info("Opening screenshot directory");

            Process.Start("explorer", Paths.GetScreenshotsDirectory(gameModule.Module));
        }

        private void OnOpenGameSettingsCommand(GameModule gameModule)
        {
            _logger.Info("Opening game settings dialog");

            DialogParameters dp = new DialogParameters
            {
                { "Title", gameModule.Title },
                { "GameSettings", gameModule.Settings }
            };

            _dialogService.ShowDialog(nameof(GameSettingsDialog), dp, null);
        }

        private async Task OnRunCommand(GameModule gameModule)
        {
            try
            {
                if (gameModule.Settings.VirtualExecutableEnabled && !string.IsNullOrWhiteSpace(gameModule.Settings.VirtualExecutablePath))
                {
                    _logger.Info($"Running virtual executable for {gameModule.Module}");

                    Process.Start(gameModule.Settings.VirtualExecutablePath);
                }
                else
                {
                    _logger.Info($"Running default executable for {gameModule.Module}");

                    Process.Start(Path.Combine(gameModule.InstalledLocation, gameModule.Executable));
                }
            }
            catch (Exception ex)
            {
                await new MessageDialog(Strings.ERROR_AN_UNKNOWN_ERROR_HAS_OCCURED).OpenAsync();
                _logger.Error(ex);
            }
        }

        private void ActivateModule(string name)
        {
            var gameModule = Games.Single(x => x.Module == name);
            gameModule.Activate();

            _configurationManager.Settings.LastActiveGame = name;
            _configurationManager.SaveSettings();

            _logger.Info($"Module '{name}' activated");
        }

        #endregion
    }
}
