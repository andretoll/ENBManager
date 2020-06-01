using ENBManager.Configuration.Interfaces;
using ENBManager.Configuration.Services;
using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.BusinessEntities.Dialogs;
using ENBManager.Infrastructure.Constants;
using ENBManager.Localization.Strings;
using ENBManager.Modules.Shared.Events;
using ENBManager.Modules.Shared.Interfaces;
using ENBManager.Modules.Shared.Models;
using ENBManager.Modules.Shared.ViewModels.Base;
using MaterialDesignThemes.Wpf;
using NLog;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ENBManager.Modules.Shared.ViewModels
{
    public class PresetsViewModel : TabItemBase
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IConfigurationManager<AppSettings> _configurationManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IPresetManager _presetManager;

        private GameModule _game;

        #endregion

        #region Public Properties

        public ObservableCollection<Preset> Presets { get; set; }

        #endregion

        #region Helper Properties

        private bool _listPresetView;
        public bool ListPresetView
        {
            get { return _listPresetView; }
            set
            {
                if (!value && !_gridPresetView)
                    value = true;

                _listPresetView = value;
                RaisePropertyChanged();

                _configurationManager.Settings.DefaultPresetView = value;
                _configurationManager.SaveSettings();
            }
        }

        private bool _gridPresetView;
        public bool GridPresetView
        {
            get { return _gridPresetView; }
            set
            {
                if (!value && !_listPresetView)
                    value = true;

                _gridPresetView = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Commands

        public DelegateCommand<Preset> ActivatePresetCommand { get; }
        public DelegateCommand<Preset> RenamePresetCommand { get; }
        public DelegateCommand<Preset> DeletePresetCommand { get; }

        #endregion

        #region Constructor

        public PresetsViewModel(
            IConfigurationManager<AppSettings> configurationManager, 
            IEventAggregator eventAggregator,
            IPresetManager presetManager)
            : base(eventAggregator)
        {
            _configurationManager = configurationManager;
            _eventAggregator = eventAggregator;
            _presetManager = presetManager;

            ActivatePresetCommand = new DelegateCommand<Preset>(async (x) => await OnActivatePresetCommand(x));
            RenamePresetCommand = new DelegateCommand<Preset>(async (x) => await OnRenamePresetCommand(x));
            DeletePresetCommand = new DelegateCommand<Preset>(async (x) => await OnDeletePresetCommand(x));

            _listPresetView = _configurationManager.Settings.DefaultPresetView;
            _gridPresetView = !_listPresetView;

            _logger.Debug($"{nameof(PresetsViewModel)} initialized");
        }

        #endregion

        #region Private Methods

        private async Task OnActivatePresetCommand(Preset preset)
        {
            var dialog = new ProgressDialog(true);
            _ = DialogHost.Show(dialog, RegionNames.RootDialogHost);

            foreach (var other in Presets.Where(x => x.Name != preset.Name))
            {
                if (other.IsActive)
                {
                    await _presetManager.DeactivatePreset(_game, other);
                    other.IsActive = false;
                }
            }

            if (preset.IsActive)
                _game.Settings.ActivePreset = preset.Name;
            else
                _game.Settings.ActivePreset = string.Empty;

            var configManager = new ConfigurationManager<GameSettings>(_game.Settings);
            configManager.SaveSettings();

            if (preset.IsActive)
            {
                await _presetManager.ActivatePreset(_game, preset);
                _eventAggregator.GetEvent<ShowSnackbarMessageEvent>().Publish($"{preset.Name} {Strings.PRESET_ACTIVATED}");
            }
            else
            {
                await _presetManager.DeactivatePreset(_game, preset);
                _eventAggregator.GetEvent<ShowSnackbarMessageEvent>().Publish(Strings.NO_PRESET_ACTIVE);
            }

            dialog.CloseDialog();
            _logger.Info("Preset activated");
        }

        private async Task OnRenamePresetCommand(Preset preset)
        {
            var dialog = new InputDialog
            {
                Message = Strings.ENTER_A_NEW_NAME,
                Value = preset.Name
            };

            var result = await DialogHost.Show(dialog, RegionNames.RootDialogHost);

            if (result != null && (bool)result)
            {
                try
                {
                    preset.FullPath = _presetManager.RenamePreset(preset, dialog.Value);
                    preset.Name = dialog.Value;

                    _logger.Info("Preset renamed");
                }
                catch (ArgumentNullException ex)
                {
                    _logger.Warn(ex);
                }
                catch (DirectoryNotFoundException ex)
                {
                    _logger.Warn(ex);
                }
                catch (ArgumentException ex)
                {
                    _logger.Debug(ex);
                    return;
                }
            }
        }

        private async Task OnDeletePresetCommand(Preset preset)
        {
            var dialog = new MessageDialog()
            {
                Message = Strings.YOU_ARE_ABOUT_TO_DELETE_THIS_ITEM_ARE_YOU_SURE
            };

            var result = await DialogHost.Show(dialog, RegionNames.RootDialogHost);

            if (result != null && (bool)result)
            {
                try
                {
                    _presetManager.DeletePreset(preset);
                    _logger.Info("Preset deleted");
                }
                catch (DirectoryNotFoundException ex)
                {
                    _logger.Warn(ex);
                }

                Presets.Remove(preset);
            }
        }

        #endregion

        #region TabItemBase Override

        public override string Name => Strings.PRESETS;

        protected override void OnModuleActivated(GameModule game)
        {
            _game = game;
            Presets = new ObservableCollection<Preset>(game.Presets);
            RaisePropertyChanged(nameof(Presets));
        } 

        #endregion
    }
}
