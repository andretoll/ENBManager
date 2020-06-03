using ENBManager.Infrastructure.Helpers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ENBManager.Infrastructure.BusinessEntities.Dialogs.Base
{
    public abstract class BaseDialog
    {
        #region Public Properties

        public string Message { get; set; } 

        #endregion

        #region Constructor

        public BaseDialog(string message)
        {
            Message = message;
        }

        #endregion

        #region Public Methods

        public bool Open()
        {
            return DialogHelper.ShowDialog(this);
        }

        public async Task<bool> OpenAsync()
        {
            return await DialogHelper.ShowDialogAsync(this);
        }

        public void Close()
        {
            DialogHelper.CloseDialog();
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
