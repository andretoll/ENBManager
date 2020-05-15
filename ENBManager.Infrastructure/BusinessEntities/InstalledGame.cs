using ENBManager.Infrastructure.BusinessEntities.Base;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ENBManager.Infrastructure.BusinessEntities
{
    public abstract class InstalledGame : GameBase, INotifyPropertyChanged
    {
        public abstract string Executable { get; }

        public bool Installed => !string.IsNullOrEmpty(InstalledLocation);

        private bool _shouldManage = true;
        public bool ShouldManage
        {
            get { return _shouldManage && Installed; }
            set
            {
                _shouldManage = value;
                OnPropertyChanged();
            }
        }

        public override string Directory => Executable.Split('.').First();

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        } 

        #endregion
    }
}
