using ENBManager.Configuration.Services;
using ENBManager.Core.Interfaces;
using ENBManager.Infrastructure.BusinessEntities;
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

        private readonly IFileService _fileService;
        private readonly IGameLocator _gameLocator;
        private readonly IGameModuleCatalog _gameModuleCatalog;

        private bool AnyGamesManaged => Games != null && Games.Any(x => x.ShouldManage);

        #endregion

        #region Public Properties

        public ObservableCollection<GameModule> Games { get; set; }

        #endregion

        #region Helper Properties

        public bool ShowUnmanagingWarning { get; set; }

        #endregion

        #region Commands

        public DelegateCommand ContinueCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand GetDataCommand { get; }
        public DelegateCommand<GameModule> BrowseGameCommand { get; }

        #endregion

        #region Constructor

        public DiscoverGamesDialogViewModel(
            IFileService fileService,
            IGameLocator gameLocator,
            IGameModuleCatalog gameModuleCatalog)
        {
            _fileService = fileService;
            _gameLocator = gameLocator;
            _gameModuleCatalog = gameModuleCatalog;

            ContinueCommand = new DelegateCommand(OnContinueCommand).ObservesCanExecute(() => AnyGamesManaged);
            CancelCommand = new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel)));
            GetDataCommand = new DelegateCommand(async () => await OnGetDataCommand());
            BrowseGameCommand = new DelegateCommand<GameModule>((p) => OnBrowseGameCommand(p));

            _logger.Debug($"{nameof(DiscoverGamesDialogViewModel)} initialized");
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
            _logger.Debug(nameof(OnContinueCommand));

            ConfigurationManager<GameSettings> configManager;
            GameSettings gameSettings;

            // For every game to manage, initialize settings
            foreach (var game in Games.Where(x => x.ShouldManage))
            {
                gameSettings = new GameSettings(game.Module)
                {
                    InstalledLocation = game.InstalledLocation
                };
                configManager = new ConfigurationManager<GameSettings>(gameSettings);
                configManager.InitializeSettings();
            }

            // For every game to not manage, delete directory
            foreach (var game in Games.Where(x => !x.ShouldManage))
            {
                // If game directory exists, delete it and its content
                var directories = _fileService.GetGameDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    if (new DirectoryInfo(directories[i]).Name == game.Module)
                    {
                        configManager = new ConfigurationManager<GameSettings>(new GameSettings(game.Module));
                        configManager.SetReadOnly(false);
                        _fileService.DeleteGameDirectory(game.Module);
                    }
                }
            }

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        private async Task OnGetDataCommand()
        {
            _logger.Debug(nameof(OnGetDataCommand));

            if (Games == null)
                Games = new ObservableCollection<GameModule>();

            var supportedGames = _gameModuleCatalog.GameModules;

            foreach (var game in supportedGames)
            {
                if (Games.Any(x => x.Module == game.Module))
                    continue;

                game.InstalledLocation = await _gameLocator.Find(game.Title);
                game.PropertyChanged += Game_PropertyChanged;
                Games.Add(game);
            }

            RaisePropertyChanged(nameof(Games));
            ContinueCommand.RaiseCanExecuteChanged();
        }

        private void OnBrowseGameCommand(GameModule game)
        {
            _logger.Debug(nameof(OnBrowseGameCommand));

            string filePath = _fileService.BrowseGameExecutable(game.Executable);

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
            _logger.Info(nameof(OnDialogClosed));
        }

        public void OnDialogOpened(IDialogParameters parameters) 
        {
            _logger.Info(nameof(OnDialogOpened));

            if (parameters.Count > 0)
            {
                ShowUnmanagingWarning = true;
                RaisePropertyChanged(nameof(ShowUnmanagingWarning));
                Games = new ObservableCollection<GameModule>(parameters.GetValue<IEnumerable<GameModule>>("Games"));
                foreach (var game in Games)
                {
                    game.PropertyChanged += Game_PropertyChanged;
                }
            }
        }

        #endregion
    }
}
