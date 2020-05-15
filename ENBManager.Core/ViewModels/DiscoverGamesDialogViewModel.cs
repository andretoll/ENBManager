using ENBManager.Configuration.Models;
using ENBManager.Configuration.Services;
using ENBManager.Core.Interfaces;
using ENBManager.Infrastructure.BusinessEntities;
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

        private readonly IFileService _fileService;
        private readonly IGameLocator _gameLocator;
        private readonly IModuleCatalog _moduleCatalog;
        private ObservableCollection<InstalledGame> _games;

        private bool _anyGamesManaged => _games != null && _games.Any(x => x.ShouldManage);
        private bool _aborted;

        #endregion

        #region Public Properties

        public ObservableCollection<InstalledGame> Games
        {
            get { return _games; }
            set
            {
                _games = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Commands

        public DelegateCommand ContinueCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand GetDataCommand { get; set; }
        public DelegateCommand<InstalledGame> BrowseGameCommand { get; set; }

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

            RequestClose += (obj) => { _aborted = obj.Result != ButtonResult.OK; };
            
            ContinueCommand = new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(ButtonResult.OK))).ObservesCanExecute(() => _anyGamesManaged);
            CancelCommand = new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel)));
            GetDataCommand = new DelegateCommand(async () => await OnGetDataCommand());
            BrowseGameCommand = new DelegateCommand<InstalledGame>((p) => OnBrowseGameCommand(p));
        }

        #endregion

        #region Events

        private void Game_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ContinueCommand.RaiseCanExecuteChanged();
        }

        #endregion

        #region Private Methods

        private async Task OnGetDataCommand()
        {
            Games = new ObservableCollection<InstalledGame>(GetInstalledGames());

            foreach (var game in Games)
            {
                game.PropertyChanged += Game_PropertyChanged;
                game.InstalledLocation = await _gameLocator.Find(game.Title);
            }

            ContinueCommand.RaiseCanExecuteChanged();
        }

        private void OnBrowseGameCommand(InstalledGame game)
        {
            string filePath = _fileService.BrowseGameExecutable(game.Executable);

            if (string.IsNullOrEmpty(filePath))
                return;
            
            game.InstalledLocation = Path.GetDirectoryName(filePath);
            game.ShouldManage = true;
            game.OnPropertyChanged(nameof(game.Installed));
            game.OnPropertyChanged(nameof(game.InstalledLocation));
        }

        private IEnumerable<InstalledGame> GetInstalledGames()
        {
            var gamesList = new List<InstalledGame>();

            foreach (var module in _moduleCatalog.Modules)
            {
                var game = (InstalledGame)Activator.CreateInstance(Type.GetType(module.ModuleType));
                gamesList.Add(game);
            }

            return gamesList;
        }

        private void InitializeGames()
        {
            if (_aborted)
                return;

            ConfigurationManager<GameSettings> configManager;

            foreach (var game in _games.Where(x => x.ShouldManage))
            {
                configManager = new ConfigurationManager<GameSettings>(new GameSettings(game.Directory));
                configManager.Initialize();
            }
        }

        #endregion

        #region IDialogAware Implementation

        public string Title { get { return Localization.Strings.Strings.INSTALLED_GAMES; } }

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            InitializeGames();
        }

        public void OnDialogOpened(IDialogParameters parameters) { }

        #endregion
    }
}
