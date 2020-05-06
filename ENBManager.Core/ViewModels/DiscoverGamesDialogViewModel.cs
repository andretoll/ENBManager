using ENBManager.Core.BusinessEntities.Base;
using ENBManager.Core.Events;
using ENBManager.Core.Interfaces;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace ENBManager.Core.ViewModels
{
    public class DiscoverGamesDialogViewModel : BindableBase, IDialogAware
    {
        #region Private Members

        private readonly IGameLocator _gameLocator;
        private readonly IGameRegistry _gameRegistry;

        #endregion

        #region Public Properties

        public GamesListViewModel GamesListViewModel { get; set; }

        #endregion

        #region Commands

        public DelegateCommand ContinueCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand GetDataCommand { get; set; }

        #endregion

        #region Constructor

        public DiscoverGamesDialogViewModel(
            IEventAggregator eventAggregator, 
            IGameLocator gameLocator, 
            IGameRegistry gameRegistry)
        {
            _gameLocator = gameLocator;
            _gameRegistry = gameRegistry;

            eventAggregator.GetEvent<GameSelectedEvent>().Subscribe(OnGameSelected);

            GamesListViewModel = new GamesListViewModel(eventAggregator);

            ContinueCommand = new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(ButtonResult.OK)));
            CancelCommand = new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel)));
            GetDataCommand = new DelegateCommand(OnGetDataCommand);
        }

        #endregion

        #region Private Methods

        private async void OnGetDataCommand()
        {
            var supportedGames = _gameRegistry.GetSupportedGames();

            foreach (var game in supportedGames)
            {
                game.InstalledLocation = await _gameLocator.Find(game.Title);
                //TODO: Apply icon
            }

            await GamesListViewModel.Initialize(supportedGames);

            //TODO: Show progressbar in the meantime
        }

        private void OnGameSelected(GameBase game)
        {

        }

        #endregion

        #region IDialogAware Implementation

        public string Title { get { return Localization.Strings.Strings.INSTALLED_GAMES; } }

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        } 

        #endregion
    }
}
