using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Localization.Strings;
using ENBManager.Modules.Shared.Events;
using ENBManager.Modules.Shared.ViewModels.Base;
using NLog;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace ENBManager.Modules.Shared.ViewModels
{
    public class DashboardViewModel : TabItemBase
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private GameModule _game;

        #endregion

        #region Properties

        public string Title => _game?.Title;
        public string InstalledLocation => _game?.InstalledLocation;
        public BitmapImage Image => _game?.Icon;
        public int? PresetCount => _game?.Presets.Count();

        public ObservableCollection<Notification> Notifications { get; set; }

        #endregion

        #region Commands

        public DelegateCommand<Notification> RemoveNotificationCommand { get; set; }

        #endregion

        #region Constructor

        public DashboardViewModel(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
            RemoveNotificationCommand = new DelegateCommand<Notification>(OnRemoveNotificationCommand);

            eventAggregator.GetEvent<PresetsCollectionChangedEvent>().Subscribe(UpdateDashboard);
        }

        #endregion

        #region Private Methods

        private void UpdateDashboard()
        {
            _logger.Debug(nameof(UpdateDashboard));

            RaisePropertyChanged(nameof(Notifications));
            RaisePropertyChanged(nameof(Title));
            RaisePropertyChanged(nameof(InstalledLocation));
            RaisePropertyChanged(nameof(Image));
            RaisePropertyChanged(nameof(PresetCount));
        }

        private void OnRemoveNotificationCommand(Notification notification)
        {
            _logger.Debug(nameof(OnRemoveNotificationCommand));

            Notifications.Remove(notification);
        }

        #endregion

        #region TabItemBase Override

        public override string Name => Strings.DASHBOARD;

        protected override void OnModuleActivated(GameModule game)
        {
            Notifications = new ObservableCollection<Notification>();

            _game = game;
            bool healthy = true;

            // Verify installation path
            if (!File.Exists(Path.Combine(game.Settings.InstalledLocation, game.Executable)))
            {
                healthy = false;
                Notifications.Add(new Notification(Icon.Error, Strings.ERROR_UNABLE_TO_LOCATE_GAME_DIRECTORY, null, null));
            }

            //TODO: Verify binaries
            if (true)
            {
            }

            //TODO: Verify active preset (compare files)
            if (true)
            {
            }

            if (healthy)
            {
                Notifications.Add(new Notification(Icon.Success, Strings.NO_PROBLEMS_HAVE_BEEN_DETECTED, null, null));
            }

            UpdateDashboard();

            _logger.Debug($"Module {game.Module} activated");
        } 

        #endregion
    }

    public class Notification
    {
        #region Public Properties

        public Icon Icon { get; set; }
        public string Message { get; set; }
        public Action Action { get; set; }
        public string ActionButtonText { get; set; }
        public bool IsActive { get; set; }

        #endregion

        #region Commands

        public DelegateCommand ExecuteActionCommand { get; }
        public DelegateCommand HideCommand { get; }

        #endregion

        #region Constructor

        public Notification(Icon icon, string message, Action action, string actionButtonText)
        {
            Icon = icon;
            Message = message;
            Action = action;
            ActionButtonText = actionButtonText;
            IsActive = true;

            ExecuteActionCommand = action == null ? null : new DelegateCommand(action);
            HideCommand = new DelegateCommand(() => IsActive = false);
        } 

        #endregion
    }

    public enum Icon
    {
        Success = 0,
        Warning = 1,
        Error = 2
    }
}
