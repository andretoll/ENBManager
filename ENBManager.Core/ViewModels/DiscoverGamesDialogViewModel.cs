using ENBManager.Configuration.Services;
using ENBManager.Core.Helpers;
using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.Helpers;
using ENBManager.Infrastructure.Interfaces;
using ENBManager.Modules.Shared.Interfaces;
using NLog;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ENBManager.Core.ViewModels
{
    public class DiscoverGamesDialogViewModel : BindableBase, IDialogAware
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IGameService _gameService;
        private readonly IGameModuleCatalog _gameModuleCatalog;

        #endregion

        #region Public Properties

        public bool AnyGamesManaged => Games != null && Games.Any(x => x.ShouldManage);

        public ObservableCollection<GameModule> Games { get; set; }

        #endregion

        #region Helper Properties

        private bool _showUnmanagingWarning;
        public bool ShowUnmanagingWarning
        {
            get { return _showUnmanagingWarning; }
            set
            {
                _showUnmanagingWarning = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Commands

        public DelegateCommand ContinueCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand GetDataCommand { get; }
        public DelegateCommand<GameModule> BrowseGameCommand { get; }

        #endregion

        #region Constructor

        public DiscoverGamesDialogViewModel(
            IGameService gameService,
            IGameModuleCatalog gameModuleCatalog)
        {
            _gameService = gameService;
            _gameModuleCatalog = gameModuleCatalog;

            ContinueCommand = new DelegateCommand(OnContinueCommand).ObservesCanExecute(() => AnyGamesManaged);
            CancelCommand = new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel)));
            GetDataCommand = new DelegateCommand(async () => await OnGetDataCommand());
            BrowseGameCommand = new DelegateCommand<GameModule>((p) => OnBrowseGameCommand(p));
        }

        #endregion

        #region Events

        private void Game_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ContinueCommand.RaiseCanExecuteChanged();
        }

        #endregion

        #region Private Methods

        private void OnContinueCommand()
        {
            _logger.Info("Continuing");

            GameSettings gameSettings;
            ConfigurationManager<GameSettings> configManager;

            // For every game to manage, initialize settings
            foreach (var game in Games.Where(x => x.ShouldManage))
            {
                _logger.Debug($"Managing {game}");
                gameSettings = new GameSettings(game.Module) { InstalledLocation = game.InstalledLocation };
                configManager = new ConfigurationManager<GameSettings>(gameSettings);
                configManager.InitializeSettings();
            }

            // For every game to not manage, delete directory
            foreach (var game in Games.Where(x => !x.ShouldManage))
            {
                _logger.Debug($"Unmanaging {game}");

                // If game directory exists, delete it and its content
                var directories = _gameService.GetGameDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    if (new DirectoryInfo(directories[i]).Name == game.Module)
                    {
                        configManager = new ConfigurationManager<GameSettings>(new GameSettings(game.Module));
                        configManager.SetReadOnly(false);
                        _gameService.DeleteGameDirectory(game.Module);
                    }
                }
            }

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        private async Task OnGetDataCommand()
        {
            _logger.Info("Getting supported games");

            if (Games == null)
                Games = new ObservableCollection<GameModule>();

            var supportedGames = _gameModuleCatalog.GameModules;

            foreach (var game in supportedGames)
            {
                if (Games.Any(x => x.Module == game.Module))
                    continue;

                game.InstalledLocation = await GameLocator.Find(game.Title);
                game.PropertyChanged += Game_PropertyChanged;
                Games.Add(game);
            }

            RaisePropertyChanged(nameof(Games));
            ContinueCommand.RaiseCanExecuteChanged();
        }

        private void OnBrowseGameCommand(GameModule game)
        {
            _logger.Info("Browsing game");

            string filePath = DialogHelper.OpenExecutable(game.Executable);

            if (string.IsNullOrEmpty(filePath))
                return;
            
            game.InstalledLocation = Path.GetDirectoryName(filePath);
            game.ShouldManage = true;
        }

        #endregion

        #region IDialogAware Implementation

        public string Title => Localization.Strings.Strings.INSTALLED_GAMES;

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() 
        {
            _logger.Info("Closed");
        }

        public void OnDialogOpened(IDialogParameters parameters) 
        {
            if (parameters.GetValue<IEnumerable<GameModule>>("Games") != null)
            {
                ShowUnmanagingWarning = true;
                Games = new ObservableCollection<GameModule>(parameters.GetValue<IEnumerable<GameModule>>("Games"));
                foreach (var game in Games)
                {
                    game.PropertyChanged += Game_PropertyChanged;
                }
            }

            _logger.Info("Opened");
        }

        #endregion
    }
}
