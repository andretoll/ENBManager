using ENBManager.Core.BusinessEntities;
using ENBManager.Core.Interfaces;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ENBManager.Core.ViewModels
{
    public class DiscoverGamesDialogViewModel : BindableBase, IDialogAware
    {
        #region Private Members

        private readonly IGameLocator _gameLocator;
        private readonly IGameRegistry _gameRegistry;
        private ObservableCollection<InstalledGame> _games;

        private bool AnyGamesManaged => _games != null && _games.Any(x => x.ShouldManage);

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

        #endregion

        #region Constructor

        public DiscoverGamesDialogViewModel(
            IGameLocator gameLocator,
            IGameRegistry gameRegistry)
        {
            _gameLocator = gameLocator;
            _gameRegistry = gameRegistry;

            ContinueCommand = new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(ButtonResult.OK))).ObservesCanExecute(() => AnyGamesManaged);
            CancelCommand = new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel)));
            GetDataCommand = new DelegateCommand(async () => await OnGetDataCommand());
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
            Games = new ObservableCollection<InstalledGame>(_gameRegistry.GetSupportedGames());

            foreach (var game in Games)
            {
                game.PropertyChanged += Game_PropertyChanged;
                game.InstalledLocation = await _gameLocator.Find(game.Title);
            }

            ContinueCommand.RaiseCanExecuteChanged();
        }


        #endregion

        #region IDialogAware Implementation

        public string Title { get { return Localization.Strings.Strings.INSTALLED_GAMES; } }

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            //TODO: For every game to manage, initialize managed directory via service
        }

        public void OnDialogOpened(IDialogParameters parameters) { }

        #endregion
    }
}
