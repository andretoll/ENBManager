using ENBManager.Configuration.Interfaces;
using ENBManager.Configuration.Services;
using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.Constants;
using ENBManager.Localization.Strings;
using ENBManager.Modules.Shared.Events;
using ENBManager.Modules.Shared.Interfaces;
using ENBManager.Modules.Shared.Models;
using ENBManager.Modules.Shared.ViewModels.Base;
using NLog;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ENBManager.Modules.Shared.ViewModels
{
    public class ScreenshotViewModel : TabItemBase
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IConfigurationManager<AppSettings> _configurationManager;
        private readonly IEventAggregator _eventAggregator;
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
                _logger.Info($"Screenshots enabled: {value}");

                _game.Settings.ScreenshotsEnabled = value;
                var config = new ConfigurationManager<GameSettings>(_game.Settings);
                config.SaveSettings();

                if (value)
                    StartCollecting();
                else
                    StopCollecting();

                RaisePropertyChanged();

                _eventAggregator.GetEvent<ScreenshotsStatusChangedModuleEvent>().Publish(value);
            }
        }

        public ObservableCollection<Preset> Categories { get; set; }

        private Preset _selectedCategory;
        public Preset SelectedCategory
        {
            get { return _selectedCategory; }
            set 
            {
                _selectedCategory = value;

                RaisePropertyChanged();
                GoToDirectoryCommand.RaiseCanExecuteChanged();

                SetScreenshotSource(value);
            }
        }

        public ObservableCollection<BitmapImage> Screenshots { get; set; }

        #endregion

        #region Commands

        public DelegateCommand LoadedCommand { get; }
        public DelegateCommand GoToDirectoryCommand { get; }

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
            _eventAggregator = eventAggregator;
            _screenshotManager = screenshotManager;
            _screenshotWatcher = screenshotWatcher;

            _eventAggregator.GetEvent<ScreenshotsStatusChangedExternalEvent>().Subscribe((x) => EnableScreenshots = x);

            LoadedCommand = new DelegateCommand(OnLoadedCommand);
            GoToDirectoryCommand = new DelegateCommand(OnGoToDirectoryCommand, () => SelectedCategory?.Screenshots?.Count > 0);
        }

        #endregion

        #region Events

        private void FileCreated(object sender, FileSystemEventArgs e)
        {
            _logger.Info("Detecting new screenshot");

            var activePreset = _game.Presets.Where(x => x.IsActive).SingleOrDefault();

            // If a preset is active, copy to preset dir
            try
            {
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
                }
            }
            catch (FileNotFoundException ex)
            {
                _logger.Warn(ex);
            }

            // Reload UI on UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                OnLoadedCommand();
            });
        }

        #endregion

        #region Private Methods

        private void OnLoadedCommand()
        {
            _logger.Debug("Loading");

            RaisePropertyChanged(nameof(EnableScreenshots));

            string selectedCategoryName = null;
            if (_selectedCategory != null)
                selectedCategoryName = _selectedCategory.Name;

            GetScreenshots();

            Categories = GetCategories();
            RaisePropertyChanged(nameof(Categories));

            SelectedCategory = null;
            if (!string.IsNullOrEmpty(selectedCategoryName))
                SelectedCategory = Categories.SingleOrDefault(x => x.Name == selectedCategoryName);
            else if (Categories != null && Categories.Count > 0)
                SelectedCategory = Categories[0];
        }

        private void OnGoToDirectoryCommand()
        {
            _logger.Info("Opening screenshot directory");

            if (!string.IsNullOrEmpty(_selectedCategory.FullPath))
                Process.Start("explorer", Paths.GetPresetScreenshotsDirectory(_game.Module, _selectedCategory.Name));
            else
                Process.Start("explorer", Paths.GetScreenshotsDirectory(_game.Module));
        }

        #endregion

        #region Helper Method

        private ObservableCollection<Preset> GetCategories()
        {
            _logger.Debug("Getting screenshot categories");

            if (_game == null)
                return null;

            var presets = new ObservableCollection<Preset>(_game.Presets);

            presets.Add(new Preset()
            {
                Name = Strings.MISC,
                Screenshots = new ObservableCollection<string>(_screenshotManager.GetScreenshots(Paths.GetScreenshotsDirectory(_game.Module)))
            });

            return presets;
        }

        private void GetScreenshots()
        {
            _logger.Debug("Getting screenshots");

            // Get screenshots for each preset
            foreach (var preset in _game.Presets)
            {
                preset.Screenshots = new ObservableCollection<string>(_screenshotManager.GetScreenshots(Paths.GetPresetScreenshotsDirectory(_game.Module, preset.Name)));
            }
        }

        private void SetScreenshotSource(Preset preset)
        {
            _logger.Info("Setting screenshot source");
            
            if (preset == null)
                return;

            Screenshots = new ObservableCollection<BitmapImage>();

            foreach (var url in preset.Screenshots)
            {
                Screenshots.Add(CreateBitmapImageFromUri(new Uri(url)));
            }

            RaisePropertyChanged(nameof(Screenshots));
        }

        private BitmapImage CreateBitmapImageFromUri(Uri source)
        {
            _logger.Debug("Creating screenshot bitmap");

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = source;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }

        private void StartCollecting()
        {
            _logger.Debug("Starting collecting");

            _screenshotWatcher.FileCreated += FileCreated;
            _screenshotWatcher.Start();
        }

        private void StopCollecting()
        {
            _logger.Debug("Stopping collecting");
            _screenshotWatcher.Stop();
        }

        #endregion

        #region TabItemBase Override

        public override string Name => Strings.SCREENSHOTS;

        protected override void OnModuleActivated(GameModule game)
        {
            _game = game;

            _screenshotWatcher.Configure(game.InstalledLocation, FileTypes.ScreenshotFileTypes);
            StopCollecting();
            
            // If screenshot watcher is enabled
            if (game.Settings.ScreenshotsEnabled)
            {
                StartCollecting();
            }

            _eventAggregator.GetEvent<ScreenshotsStatusChangedModuleEvent>().Publish(_game.Settings.ScreenshotsEnabled);
        }

        #endregion
    }
}
