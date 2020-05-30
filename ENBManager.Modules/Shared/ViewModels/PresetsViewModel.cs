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
using Prism.Commands;
using Prism.Events;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ENBManager.Modules.Shared.ViewModels
{
    public class PresetsViewModel : TabItemBase
    {
        #region Private Members

        private readonly IConfigurationManager<AppSettings> _configurationManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IPresetManager _presetManager;

        private GameModule _game;

        #endregion

        #region Public Properties

        public IEnumerable<Preset> Presets => _game?.Presets;

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

            ActivatePresetCommand = new DelegateCommand<Preset>(OnActivatePresetCommand);
            RenamePresetCommand = new DelegateCommand<Preset>(async (x) => await OnRenamePresetCommand(x));

            _listPresetView = _configurationManager.Settings.DefaultPresetView;
            _gridPresetView = !_listPresetView;
        }

        #endregion

        #region Private Methods

        private void OnActivatePresetCommand(Preset preset)
        {
            foreach (var other in Presets.Where(x => x.Name != preset.Name))
            {
                other.IsActive = false;
            }

            if (preset.IsActive)
                _game.Settings.ActivePreset = preset.Name;
            else
                _game.Settings.ActivePreset = string.Empty;

            var configManager = new ConfigurationManager<GameSettings>(_game.Settings);
            configManager.SaveSettings();

            if (preset.IsActive)
                _eventAggregator.GetEvent<ShowSnackbarMessageEvent>().Publish($"{preset.Name} {Strings.PRESET_ACTIVATED}");
            else
                _eventAggregator.GetEvent<ShowSnackbarMessageEvent>().Publish(Strings.NO_PRESET_ACTIVE);
        }

        private async Task OnRenamePresetCommand(Preset preset)
        {
            var dialog = new EnterTextDialog
            {
                Message = Strings.RENAME,
                Value = preset.Name
            };

            var result = await DialogHost.Show(dialog, RegionNames.RootDialogHost);

            if ((bool)result)
            {
                _presetManager.RenamePreset(preset, dialog.Value);
                preset.Name = dialog.Value;
            }
        }

        #endregion

        #region TabItemBase Override

        public override string Name => Strings.PRESETS;

        protected override void OnModuleActivated(GameModule game)
        {
            _game = game;
            RaisePropertyChanged(nameof(Presets));
        } 

        #endregion
    }
}
