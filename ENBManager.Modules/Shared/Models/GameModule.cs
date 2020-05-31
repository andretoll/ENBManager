using ENBManager.Infrastructure.Constants;
using ENBManager.Modules.Shared.Events;
using ENBManager.Modules.Shared.Interfaces;
using ENBManager.Modules.Shared.Models;
using ENBManager.Modules.Shared.Views;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace ENBManager.Infrastructure.BusinessEntities
{
    public abstract class GameModule : INotifyPropertyChanged
    {
        #region Private Members

        private readonly IContainerProvider _container;

        private string _installedLocation;
        private bool _shouldManage = true;
        private GameSettings _settings;

        #endregion

        #region Public Properties

        public string InstalledLocation
        {
            get { return _installedLocation; }
            set
            {
                _installedLocation = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Installed));
            }
        }
        public bool Installed => !string.IsNullOrEmpty(InstalledLocation);
        public bool ShouldManage
        {
            get { return _shouldManage && Installed; }
            set
            {
                _shouldManage = value;
                OnPropertyChanged();
            }
        }
        public IEnumerable<Preset> Presets { get; set; }

        public GameSettings Settings
        {
            get { return _settings; }
            set
            {
                _settings = value;

                InstalledLocation = value.InstalledLocation;
            }
        }

        #endregion

        #region Public Abstract Properties

        public abstract string Title { get; }
        public abstract BitmapImage Icon { get; }
        public abstract string Executable { get; } 
        public abstract string Module { get; }
        public abstract string[] Binaries { get; }

        #endregion

        #region Constructor

        public GameModule(IContainerProvider container)
        {
            _container = container;
        }

        #endregion

        #region Protected Methods

        protected void ActivateModule(params Type[] types)
        {
            // Get presets and active preset
            var presetManager = _container.Resolve<IPresetManager>();
            Presets = presetManager.GetPresets(Paths.GetPresetsDirectory(Module));
            if (Presets != null && Presets.Count() > 0)
            {
                var activePreset = Presets.FirstOrDefault(x => x.Name == Settings.ActivePreset);

                if (activePreset != null)
                    activePreset.IsActive = true;
            }

            // Register views
            var regionManager = _container.Resolve<IRegionManager>();
            regionManager.RequestNavigate(RegionNames.MainRegion, nameof(ModuleShell));
            regionManager.Regions[RegionNames.TabRegion].RemoveAll();
            foreach (var type in types)
            {
                regionManager.RegisterViewWithRegion(RegionNames.TabRegion, type);
            }

            // Publish event
            var eventAggregator = _container.Resolve<IEventAggregator>();
            eventAggregator.GetEvent<ModuleActivatedEvent>().Publish(this);
        }

        #endregion

        #region Public Abstract Methods

        public abstract void Activate();

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
