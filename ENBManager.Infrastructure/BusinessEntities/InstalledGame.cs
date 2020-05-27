using ENBManager.Infrastructure.Constants;
using Prism.Regions;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace ENBManager.Infrastructure.BusinessEntities
{
    public abstract class InstalledGame : INotifyPropertyChanged
    {
        #region Private Members

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

        #endregion

        #region Public Abstract Methods

        public virtual void Activate(IRegionManager regionManager)
        {
            regionManager.Regions[RegionNames.TabRegion].RemoveAll();
        }

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
