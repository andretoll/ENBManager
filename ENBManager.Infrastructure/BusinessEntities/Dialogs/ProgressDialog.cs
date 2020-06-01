using MaterialDesignThemes.Wpf;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ENBManager.Infrastructure.BusinessEntities.Dialogs
{
    public class ProgressDialog : INotifyPropertyChanged
    {
        #region Private Members

        private int _progress;

        #endregion

        #region Public Properties

        public string Message { get; set; }

        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }

        public bool IsIndeterminate { get; set; }

        #endregion

        #region Constructor

        public ProgressDialog(bool isIndeterminate)
        {
            IsIndeterminate = isIndeterminate;

            if (isIndeterminate)
                Progress = 0;
        }

        #endregion

        #region Public Methods

        public void CloseDialog()
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
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
