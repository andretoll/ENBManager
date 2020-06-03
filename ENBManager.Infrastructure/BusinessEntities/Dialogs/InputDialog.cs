using ENBManager.Infrastructure.BusinessEntities.Dialogs.Base;
using System.ComponentModel;

namespace ENBManager.Infrastructure.BusinessEntities.Dialogs
{
    public class InputDialog : BaseDialog, INotifyPropertyChanged
    {
        #region Private Members

        private string _value;

        #endregion

        #region Public Properties

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

        public InputDialog(string message, string value = "")
            : base (message) 
        {
            Value = value;
        }

        #endregion
    }
}
