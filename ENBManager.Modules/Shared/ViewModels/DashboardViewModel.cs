using ENBManager.Configuration.Services;
using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.Interfaces;
using ENBManager.Localization.Strings;
using ENBManager.Modules.Shared.Events;
using ENBManager.Modules.Shared.Interfaces;
using ENBManager.Modules.Shared.ViewModels.Base;
using NLog;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace ENBManager.Modules.Shared.ViewModels
{
    public class DashboardViewModel : TabItemBase
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IGameService _gameService;
        private readonly IPresetManager _presetManager;

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

        public DelegateCommand OnLoadedCommand { get; set; }
        public DelegateCommand<Notification> RemoveNotificationCommand { get; set; }

        #endregion

        #region Constructor

        public DashboardViewModel(
            IEventAggregator eventAggregator, 
            IGameService gameService,
            IPresetManager presetManager)
            : base(eventAggregator)
        {
            _gameService = gameService;
            _presetManager = presetManager;

            RemoveNotificationCommand = new DelegateCommand<Notification>(OnRemoveNotificationCommand);
            OnLoadedCommand = new DelegateCommand(VerifyIntegrity);

            eventAggregator.GetEvent<PresetsCollectionChangedEvent>().Subscribe(UpdateUI);
        }

        #endregion

        #region Private Methods

        private void UpdateUI()
        {
            _logger.Debug(nameof(UpdateUI));

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

        private void VerifyIntegrity()
        {
            _logger.Debug(nameof(VerifyIntegrity));

            Notifications.Clear();

            bool healthy = true;

            // Verify installation path
            if (!Directory.Exists(_game.Settings.InstalledLocation))
            {
                _logger.Info(Strings.ERROR_UNABLE_TO_LOCATE_GAME_DIRECTORY);

                healthy = false;
                Notifications.Add(new Notification(Icon.Error, Strings.ERROR_UNABLE_TO_LOCATE_GAME_DIRECTORY, BrowseGameDirectory, Strings.BROWSE));
            }

            // Verify binaries
            var missingFiles = _gameService.VerifyBinaries(_game.Settings.InstalledLocation, _game.Binaries);
            if (missingFiles.Length > 0)
            {
                _logger.Info(Strings.ERROR_MISSING_BINARIES);

                healthy = false;
                Notifications.Add(new Notification(Icon.Error, $"{Strings.ERROR_MISSING_BINARIES} ({string.Join(", ", missingFiles)})", OpenLink, Strings.GO_TO_ENBDEV));
            }

            // Verify active preset (compare files)
            if (_game.Presets.Count > 0 && _game.Presets.SingleOrDefault(x => x.IsActive) != null)
            {
                try
                {
                    bool valid = _presetManager.ValidatePreset(_game, _game.Presets.Single(x => x.IsActive));

                    if (!valid)
                    {
                        Notifications.Add(new Notification(Icon.Warning, Strings.WARNING_THE_ACTIVE_PRESET_DIFFERS_FROM_THE_PRESET_CURRENTLY_USED, UpdatePreset, Strings.UPDATE));
                    }
                }
                catch (FileNotFoundException ex)
                {
                    _logger.Info(ex);
                    Notifications.Add(new Notification(Icon.Warning, Strings.WARNING_FILES_ARE_MISSING_FROM_THE_ACTIVE_PRESET, UpdatePreset, Strings.UPDATE));
                }
            }

            if (healthy)
            {
                _logger.Info(Strings.NO_PROBLEMS_HAVE_BEEN_DETECTED);

                Notifications.Add(new Notification(Icon.Success, Strings.NO_PROBLEMS_HAVE_BEEN_DETECTED, null, null));
            }
        }

        private void BrowseGameDirectory()
        {
            string newPath = _gameService.BrowseGameExecutable(_game.Executable);

            if (string.IsNullOrEmpty(newPath))
                return;

            _game.Settings.InstalledLocation = Path.GetDirectoryName(newPath);

            ConfigurationManager<GameSettings> config = new ConfigurationManager<GameSettings>(_game.Settings);
            config.SaveSettings();

            VerifyIntegrity();
        }

        private void OpenLink()
        {
            string url = "http://enbdev.com/download.html";

            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };

            Process.Start(psi);
        }

        private void UpdatePreset()
        {
            var activePreset = _game.Presets.SingleOrDefault(x => x.IsActive);

            if (activePreset == null)
                return;

            _presetManager.UpdatePresetFiles(_game, activePreset);

            activePreset.Files = _presetManager.GetPresetAsync(_game, activePreset.Name).Result.Files;

            VerifyIntegrity();
        }

        #endregion

        #region TabItemBase Override

        public override string Name => Strings.DASHBOARD;

        protected override void OnModuleActivated(GameModule game)
        {
            Notifications = new ObservableCollection<Notification>();

            _game = game;

            UpdateUI();

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
