using ENBManager.Infrastructure.BusinessEntities.Base;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ENBManager.Infrastructure.BusinessEntities
{
    public abstract class InstalledGame : GameBase, INotifyPropertyChanged
    {
        #region Private Members

        private bool _shouldManage = true;

        #endregion

        #region Public Properties

        public string InstalledLocation { get; set; }
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

        #endregion

        #region Public Abstract Properties

        public abstract string Executable { get; } 
        public abstract string Module { get; }

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
