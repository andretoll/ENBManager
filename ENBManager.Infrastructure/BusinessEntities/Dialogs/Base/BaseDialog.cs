using MaterialDesignThemes.Wpf;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ENBManager.Infrastructure.BusinessEntities.Dialogs.Base
{
    public abstract class BaseDialog
    {
        #region Private Members

        private string _hostIdentifier;

        #endregion

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

        /// <summary>
        /// Sets the target <see cref="DialogHost"/> unique identifier.
        /// </summary>
        /// <param name="hostIdentifier"></param>
        public void SetHost(string hostIdentifier)
        {
            _hostIdentifier = hostIdentifier;
        }

        public bool Open()
        {
            Close();
            var result = DialogHost.Show(this, _hostIdentifier).Result;

            return result != null && (bool)result;
        }

        public async Task<bool> OpenAsync()
        {
            await CloseAsync();
            var result = await DialogHost.Show(this, _hostIdentifier);

            return result != null && (bool)result;
        }

        public void Close()
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        public Task CloseAsync()
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
            return Task.CompletedTask;
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
