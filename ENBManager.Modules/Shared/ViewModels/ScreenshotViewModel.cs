using ENBManager.Configuration.Interfaces;
using ENBManager.Configuration.Services;
using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.Constants;
using ENBManager.Localization.Strings;
using ENBManager.Modules.Shared.Interfaces;
using ENBManager.Modules.Shared.ViewModels.Base;
using NLog;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.Collections.Generic;
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

        private List<string> _screenshots;
        private ScreenshotCategory _selectedCategory;
        private ICollection<string> _miscScreenshots;

        #endregion

        #region Public Properties

        public bool EnableScreenshots
        {
            get { return _game != null && _game.Settings.ScreenshotsEnabled; }
            set
            {
                _logger.Info($"Screenshots enabled: {value}");

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

        public List<ScreenshotCategory> Categories { get; set; }

        public List<string> Screenshots
        {
            get { return _screenshots; }
            set
            {
                _screenshots = value;
                RaisePropertyChanged();
            }
        }

        public ScreenshotCategory SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                RaisePropertyChanged();

                SetScreenshotSource(value);
            }
        }

        #endregion

        #region Commands

        public DelegateCommand LoadedCommand { get; }

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

            LoadedCommand = new DelegateCommand(OnLoadedCommand);
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
                _logger.Info("Copying screenshot to preset");
                _screenshotManager.SaveScreenshot(Paths.GetPresetScreenshotsDirectory(_game.Module, activePreset.Name), e.FullPath);
                activePreset.Screenshots.Add(e.FullPath);
            }
            // Else, copy to base dir
            else if (_configurationManager.Settings.EnableScreenshotWithoutPreset)
            {
                _logger.Info("Copying screenshot to base");
                _screenshotManager.SaveScreenshot(Paths.GetScreenshotsDirectory(_game.Module), e.FullPath);
                _miscScreenshots.Add(e.FullPath);
            }

            OnLoadedCommand();
        }

        #endregion

        #region Private Methods

        private void OnLoadedCommand()
        {
            _logger.Debug("Loaded");

            GetScreenshots();
            SetScreenshotCategories();

            SelectedCategory = Categories[0];

            UpdateUI();
        }

        #endregion

        #region Helper Method

        private void UpdateUI()
        {
            _logger.Debug("Updating UI");

            RaisePropertyChanged(nameof(EnableScreenshots));
            RaisePropertyChanged(nameof(Categories));
        }

        private void GetScreenshots()
        {
            _logger.Info("Getting screenshots");

            // Get screenshots for each preset
            foreach (var preset in _game.Presets)
            {
                preset.Screenshots = _screenshotManager.GetScreenshots(Paths.GetPresetScreenshotsDirectory(_game.Module, preset.Name));
            }

            // Get misc screenshots
            _miscScreenshots = _screenshotManager.GetScreenshots(Paths.GetScreenshotsDirectory(_game.Module));
        }

        private void SetScreenshotSource(ScreenshotCategory screenshotCategory)
        {
            _logger.Info("Setting screenshot source");

            if (screenshotCategory == null)
                return;

            var preset = _game.Presets.SingleOrDefault(x => x.Name == screenshotCategory.Name);

            if (preset != null)
                Screenshots = new List<string>(preset.Screenshots);
            else
                Screenshots = new List<string>(_miscScreenshots);
        }

        private void SetScreenshotCategories()
        {
            _logger.Debug("Updating screenshot categories");

            if (_game == null)
                return;

            Categories = new List<ScreenshotCategory>();

            // Add active preset
            var activePreset = _game.Presets.SingleOrDefault(x => x.IsActive);
            if (activePreset != null)
            {
                Categories.Add(new ScreenshotCategory(activePreset.Name, activePreset.Screenshots != null ? activePreset.Screenshots.Count : 0, true));
            }

            // Add all other presets
            foreach (var preset in _game.Presets.Where(x => !x.IsActive))
            {
                Categories.Add(new ScreenshotCategory(preset.Name, preset.Screenshots != null ? preset.Screenshots.Count : 0));
            }

            // Add unrelated
            Categories.Add(new ScreenshotCategory(Strings.MISC, _miscScreenshots != null ? _miscScreenshots.Count : 0));
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
        }

        #endregion
    }

    public class ScreenshotCategory
    {
        #region Public Properties

        public string Name { get; set; }
        public int Count { get; set; }
        public bool Highlight { get; set; }

        #endregion

        #region Constructor

        public ScreenshotCategory(string name, int count, bool highlight = false)
        {
            Name = name;
            Count = count;
            Highlight = highlight;
        } 

        #endregion
    }
}
