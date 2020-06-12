using ENBManager.Configuration.Interfaces;
using ENBManager.Configuration.Services;
using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.BusinessEntities.Dialogs;
using ENBManager.Infrastructure.Constants;
using ENBManager.Infrastructure.Enums;
using ENBManager.Infrastructure.Helpers;
using ENBManager.Infrastructure.Interfaces;
using ENBManager.Localization.Strings;
using ENBManager.Modules.Shared.Events;
using ENBManager.Modules.Shared.Interfaces;
using ENBManager.Modules.Shared.Models;
using ENBManager.Modules.Shared.ViewModels.Base;
using NLog;
using Prism.Commands;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ENBManager.Modules.Shared.ViewModels
{
    public class DashboardViewModel : TabItemBase
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IConfigurationManager<AppSettings> _configurationManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IGameService _gameService;
        private readonly IPresetManager _presetManager;

        private GameModule _game;

        #endregion

        #region Properties

        public string Title => _game?.Title;
        public string InstalledLocation => _game?.InstalledLocation;
        public BitmapImage Image => _game?.Icon;
        public int? PresetCount => _game?.Presets.Count();
        public string ActivePreset => _game?.Presets.SingleOrDefault(x => x.IsActive)?.Name;

        public ObservableCollection<Notification> Notifications { get; set; }

        #endregion

        #region Commands

        public DelegateCommand LoadedCommand { get; set; }
        public DelegateCommand<Notification> RemoveNotificationCommand { get; set; }

        #endregion

        #region Constructor

        public DashboardViewModel(
            ConfigurationManager<AppSettings> configurationManager,
            IEventAggregator eventAggregator, 
            IGameService gameService,
            IPresetManager presetManager)
            : base(eventAggregator)
        {
            _configurationManager = configurationManager;
            _eventAggregator = eventAggregator;
            _gameService = gameService;
            _presetManager = presetManager;

            RemoveNotificationCommand = new DelegateCommand<Notification>(OnRemoveNotificationCommand);
            LoadedCommand = new DelegateCommand(async () => await OnLoadedCommand());
        }

        #endregion

        #region Private Methods

        private async Task OnLoadedCommand()
        {
            _logger.Info("Loaded");

            Notifications = new ObservableCollection<Notification>();
            UpdateUI();

            await VerifyIntegrity();
        }

        private void UpdateUI()
        {
            _logger.Debug("Updating UI");

            RaisePropertyChanged(nameof(Notifications));
            RaisePropertyChanged(nameof(Title));
            RaisePropertyChanged(nameof(InstalledLocation));
            RaisePropertyChanged(nameof(Image));
            RaisePropertyChanged(nameof(PresetCount));
            RaisePropertyChanged(nameof(ActivePreset));
        }

        private void OnRemoveNotificationCommand(Notification notification)
        {
            _logger.Debug("Removing notification");

            Notifications.Remove(notification);
        }

        private async Task VerifyIntegrity()
        {
            _logger.Info("Verifying integrity");

            Notifications.Clear();

            bool healthy = true;

            // Verify installation path
            if (!await VerifyInstallationPath())
            {
                _logger.Warn(Strings.ERROR_UNABLE_TO_LOCATE_GAME_DIRECTORY);

                healthy = false;
                Notifications.Add(new Notification(Icon.Error, Strings.ERROR_UNABLE_TO_LOCATE_GAME_DIRECTORY, async () => await BrowseGameDirectory(), Strings.BROWSE));
            }

            // Verify binaries
            if (ActivePreset != null && !await VerifyBinaries(out string[] missingFiles))
            {
                _logger.Warn(Strings.ERROR_MISSING_BINARIES);

                healthy = false;

                if (_configurationManager.Settings.ManageBinaries && await VerifyBinariesBackup())
                    Notifications.Add(new Notification(Icon.Error, $"{Strings.ERROR_MISSING_BINARIES} ({string.Join(", ", missingFiles)})", async () => await RestoreBinaries(), Strings.RESTORE));
                else
                    Notifications.Add(new Notification(Icon.Error, $"{Strings.ERROR_MISSING_BINARIES} ({string.Join(", ", missingFiles)})", OpenLink, Strings.VISIT_ENBDEV));
            }
            else if (_configurationManager.Settings.ManageBinaries && !await VerifyBinariesBackup())
            {
                _logger.Warn(Strings.WARNING_NO_BINARIES_BACKUP);

                healthy = false;
                Notifications.Add(new Notification(Icon.Warning, Strings.WARNING_NO_BINARIES_BACKUP, async () => await BackupBinaries(), Strings.BACKUP));
            }
            else if (_configurationManager.Settings.ManageBinaries)
            {
                var versionMismatch = await VerifyBinariesVersion();

                if (versionMismatch != VersionMismatch.Matching)
                {
                    healthy = false;

                    switch (versionMismatch)
                    {
                        // If older version is used in game dir
                        case VersionMismatch.Above:
                            _logger.Warn("Version mismatch: older version in game directory");
                            Notifications.Add(new Notification(Icon.Warning, Strings.WARNING_AN_OLDER_BINARY_VERSION_IS_CURRENTLY_USED, async () => await RestoreBinaries(), Strings.UPDATE));
                            break;
                        // If newer version is used in game dir
                        case VersionMismatch.Below:
                            _logger.Warn("Version mismatch: newer version in game directory");
                            Notifications.Add(new Notification(Icon.Warning, Strings.WARNING_A_NEWER_BINARY_VERSION_IS_CURRENTLY_USED, async () => await BackupBinaries(), Strings.UPDATE));
                            break;
                    } 
                }
            }

            // Verify active preset (compare files)
            if (_game.Presets.Count > 0 && _game.Presets.SingleOrDefault(x => x.IsActive) != null)
            {
                try
                {
                    if (!await VerifyActivePreset())
                    {
                        _logger.Info(Strings.WARNING_THE_ACTIVE_PRESET_DIFFERS_FROM_THE_PRESET_CURRENTLY_USED);
                        Notifications.Add(new Notification(Icon.Warning, Strings.WARNING_THE_ACTIVE_PRESET_DIFFERS_FROM_THE_PRESET_CURRENTLY_USED, async () => await UpdatePreset(), Strings.UPDATE));
                    }
                }
                catch (FileNotFoundException)
                {
                    _logger.Info(Strings.WARNING_FILES_ARE_MISSING_FROM_THE_ACTIVE_PRESET);
                    Notifications.Add(new Notification(Icon.Warning, Strings.WARNING_FILES_ARE_MISSING_FROM_THE_ACTIVE_PRESET, async () => await UpdatePreset(), Strings.UPDATE));
                }
            }

            if (healthy)
            {
                _logger.Info("Game healthy");

                Notifications.Add(new Notification(Icon.Success, Strings.NO_PROBLEMS_HAVE_BEEN_DETECTED, null, null));
            }
        }

        private async Task BrowseGameDirectory()
        {
            _logger.Info("Browsing new directory");

            string newPath = DialogHelper.OpenExecutable(_game.Executable);

            if (string.IsNullOrEmpty(newPath))
                return;

            _game.Settings.InstalledLocation = Path.GetDirectoryName(newPath);

            ConfigurationManager<GameSettings> config = new ConfigurationManager<GameSettings>(_game.Settings);
            config.SaveSettings();

            await VerifyIntegrity();

            _eventAggregator.GetEvent<ShowSnackbarMessageEvent>().Publish(Strings.GAME_UPDATED);
        }

        private void OpenLink()
        {
            string url = "http://enbdev.com/download.html";

            _logger.Info($"Opening {url}");

            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };

            Process.Start(psi);
        }

        private async Task UpdatePreset()
        {
            _logger.Info("Updating preset");

            using (var dialog = new ProgressDialog(true))
            {
                _ = dialog.OpenAsync();

                var activePreset = _game.Presets.SingleOrDefault(x => x.IsActive);
                if (activePreset == null)
                    return;

                await _presetManager.UpdatePresetFilesAsync(_game.InstalledLocation, activePreset);

                activePreset.Files = _presetManager.GetPresetAsync(Paths.GetPresetsDirectory(_game.Module), activePreset.Name).Result.Files;

                _eventAggregator.GetEvent<ShowSnackbarMessageEvent>().Publish(Strings.PRESET_UPDATED);
            }

            await VerifyIntegrity();
        }

        private async Task BackupBinaries()
        {
            _logger.Info("Backing up binaries");

            try
            {
                _gameService.CopyBinaries(_game.Settings.InstalledLocation, Paths.GetBinariesBackupDirectory(_game.Module), _game.Binaries);
            }
            catch (FileNotFoundException ex)
            {
                _logger.Warn(ex.Message);

                await new MessageDialog(ex.Message).OpenAsync();
            }

            await VerifyIntegrity();
        }

        private async Task RestoreBinaries()
        {
            _logger.Info("Restoring binaries");

            _gameService.CopyBinaries(Paths.GetBinariesBackupDirectory(_game.Module), _game.Settings.InstalledLocation, _game.Binaries);

            await VerifyIntegrity();
        }

        #endregion

        #region Helper Methods

        private Task<bool> VerifyInstallationPath()
        {
            _logger.Debug("Verifying installation path");

            if (!Directory.Exists(_game.Settings.InstalledLocation))
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private Task<bool> VerifyBinaries(out string[] binaries)
        {
            _logger.Debug("Verifying binaries");

            return Task.FromResult(_gameService.VerifyBinaries(_game.Settings.InstalledLocation, _game.Binaries, out binaries));
        }

        private Task<VersionMismatch> VerifyBinariesVersion()
        {
            _logger.Debug("Verifying binaries version");

            return Task.FromResult(_gameService.VerifyBinariesVersion(Paths.GetBinariesBackupDirectory(_game.Module), _game.InstalledLocation, _game.Binaries));
        }

        private Task<bool> VerifyBinariesBackup()
        {
            _logger.Debug("Verifying binaries backup");

            bool existing = _gameService.VerifyBinaries(Paths.GetBinariesBackupDirectory(_game.Module), _game.Binaries);

            return Task.FromResult(existing);
        }

        private async Task<bool> VerifyActivePreset()
        {
            _logger.Debug("Verifying active preset");

            return await _presetManager.ValidatePresetAsync(_game.InstalledLocation, _game.Presets.Single(x => x.IsActive));
        }

        #endregion

        #region TabItemBase Override

        public override string Name => Strings.DASHBOARD;

        protected override void OnModuleActivated(GameModule game)
        {
            _game = game;
        } 

        #endregion
    }
}
