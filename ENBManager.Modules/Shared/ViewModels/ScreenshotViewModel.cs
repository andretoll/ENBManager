using ENBManager.Configuration.Interfaces;
using ENBManager.Configuration.Services;
using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.Constants;
using ENBManager.Localization.Strings;
using ENBManager.Modules.Shared.Interfaces;
using ENBManager.Modules.Shared.ViewModels.Base;
using NLog;
using Prism.Events;
using System;
using System.Linq;

namespace ENBManager.Modules.Shared.ViewModels
{
    public class ScreenshotViewModel : TabItemBase
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IConfigurationManager<AppSettings> _configurationManager;
        private readonly IScreenshotManager _screenshotManager;
        private readonly IScreenshotWatcher _screenshotWatcher;

        private GameModule _game;

        #endregion

        #region Public Properties

        public bool EnableScreenshots
        {
            get { return _game != null && _game.Settings.ScreenshotsEnabled; }
            set
            {
                _game.Settings.ScreenshotsEnabled = value;
                var config = new ConfigurationManager<GameSettings>(_game.Settings);
                config.SaveSettings();

                if (value)
                    _screenshotWatcher.Start();
                else
                    _screenshotWatcher.Stop();

                RaisePropertyChanged();
            }
        }

        //TODO: Add configurable source of screenshots. InstalledLocation as default.

        #endregion

        #region Constructor

        public ScreenshotViewModel(
            IConfigurationManager<AppSettings> configurationManager,
            IEventAggregator eventAggregator,
            IScreenshotManager screenshotManager,
            IScreenshotWatcher screenshotWatcher)
            :base (eventAggregator)
        {
            _configurationManager = configurationManager;
            _screenshotManager = screenshotManager;
            _screenshotWatcher = screenshotWatcher;
        }

        #endregion

        #region Events

        private void FileCreated(object sender, System.IO.FileSystemEventArgs e)
        {
            _logger.Info("Detecting new screenshot");

            var activePreset = _game.Presets.Where(x => x.IsActive).SingleOrDefault();

            // If a preset is active, copy to preset dir
            if (activePreset != null)
            {
                try
                {
                    _logger.Info("Copying screenshot to preset");
                    _screenshotManager.SaveScreenshot(Paths.GetPresetScreenshotsDirectory(_game.Module, activePreset.Name), e.FullPath);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            }
            // Else, copy to base dir
            else if (_configurationManager.Settings.EnableScreenshotWithoutPreset)
            {
                try
                {
                    _logger.Info("Copying screenshot to base");
                    _screenshotManager.SaveScreenshot(Paths.GetScreenshotsDirectory(_game.Module), e.FullPath);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            }
        }

        #endregion

        #region Private Methods

        private void UpdateUI()
        {
            _logger.Debug("Updating UI");

            RaisePropertyChanged(nameof(EnableScreenshots));
        }

        #endregion

        #region TabItemBase Override

        public override string Name => Strings.SCREENSHOTS;

        protected override void OnModuleActivated(GameModule game)
        {
            _game = game;

            _screenshotWatcher.Configure(game.InstalledLocation, FileTypes.ScreenshotFileTypes);
            _screenshotWatcher.Stop();
            
            // If screenshot watcher is enabled
            if (game.Settings.ScreenshotsEnabled)
            {
                _screenshotWatcher.FileCreated += FileCreated;
                _screenshotWatcher.Start();
            }

            UpdateUI();
        }

        #endregion
    }
}
