using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ENBManager.Modules.Shared.Models
{
    public class Preset : INotifyPropertyChanged
    {
        #region Private Members

        private bool _isActive;

        #endregion

        #region Public Properties

        public string Name { get; set; }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<string> Files { get; set; } 

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
