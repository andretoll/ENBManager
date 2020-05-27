using ENBManager.Core.Helpers;
using ENBManager.Core.Interfaces;
using ENBManager.Core.Services;
using ENBManager.Infrastructure.BusinessEntities;
using NLog;
using Prism.Commands;
using Prism.Modularity;
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

        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IFileService _fileService;
        private readonly IGameLocator _gameLocator;
        private readonly IModuleCatalog _moduleCatalog;

        private bool _anyGamesManaged => Games != null && Games.Any(x => x.ShouldManage);

        #endregion

        #region Public Properties

        public ObservableCollection<GameModule> Games { get; set; }

        #endregion

        #region Helper Properties

        public bool ShowUnmanagingWarning { get; set; }

        #endregion

        #region Commands

        public DelegateCommand ContinueCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand GetDataCommand { get; set; }
        public DelegateCommand<GameModule> BrowseGameCommand { get; set; }

        #endregion

        #region Constructor

        public DiscoverGamesDialogViewModel(
            IFileService fileService,
            IGameLocator gameLocator,
            IModuleCatalog moduleCatalog)
        {
            _fileService = fileService;
            _gameLocator = gameLocator;
            _moduleCatalog = moduleCatalog;

            ContinueCommand = new DelegateCommand(OnContinueCommand).ObservesCanExecute(() => _anyGamesManaged);
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
            _logger.Debug(nameof(OnContinueCommand));

            ConfigurationManager<GameSettings> configManager;
            GameSettings gameSettings;

            // For every game to manage
            foreach (var game in Games.Where(x => x.ShouldManage))
            {
                gameSettings = new GameSettings(game.Module);
                gameSettings.InstalledLocation = game.InstalledLocation;
                configManager = new ConfigurationManager<GameSettings>(gameSettings);
                configManager.Initialize();
            }

            // For every game to not manage
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

            var supportedGames = await GetSupportedGames();

            foreach (var game in supportedGames)
            {
                if (Games.Any(x => x.Module == game.Module))
                    continue;

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

        private async Task<IEnumerable<GameModule>> GetSupportedGames()
        {
            _logger.Debug(nameof(GetSupportedGames));

            var gamesList = new List<GameModule>();

            foreach (var module in _moduleCatalog.Modules)
            {
                var game = (GameModule)InstanceFactory.CreateInstance(Type.GetType(module.ModuleType));
                game.InstalledLocation = await _gameLocator.Find(game.Title);
                gamesList.Add(game);
            }

            return gamesList;
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
