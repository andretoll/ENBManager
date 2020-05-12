﻿using ENBManager.Core.BusinessEntities.Base;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ENBManager.Core.BusinessEntities
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

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        } 

        #endregion
    }
}