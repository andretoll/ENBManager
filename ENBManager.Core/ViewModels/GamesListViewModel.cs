using ENBManager.Core.BusinessEntities.Base;
using Prism.Events;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ENBManager.Core.ViewModels
{
    public class GamesListViewModel : BindableBase
    {
        #region Private Members

        private readonly IEventAggregator _eventAggregator;

        private ObservableCollection<GameBase> _games;

        #endregion

        #region Public Properties

        public ObservableCollection<GameBase> Games
        {
            get { return _games; }
            set
            {
                _games = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Constructor

        public GamesListViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            Games = new ObservableCollection<GameBase>();
        }

        #endregion

        #region Public Methods

        public Task Initialize(IEnumerable<GameBase> games)
        {
            foreach (var game in games)
            {
                Games.Add(game);
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
