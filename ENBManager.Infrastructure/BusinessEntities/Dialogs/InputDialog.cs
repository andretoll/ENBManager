using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ENBManager.Infrastructure.BusinessEntities.Dialogs
{
    public class InputDialog : INotifyPropertyChanged
    {
        #region Private Members

        private string _value;

        #endregion

        #region Public Properties

        public string Message { get; set; }
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Valid));
            }
        }
        public bool Valid => !string.IsNullOrEmpty(Value);

        #endregion

        #region Constructor

        public InputDialog(string message)
        {
            Message = message;
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
