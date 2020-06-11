using ENBManager.Modules.Shared.Events;
using MaterialDesignThemes.Wpf;
using NLog;
using Prism.Events;
using Prism.Mvvm;

namespace ENBManager.Modules.Shared.ViewModels
{
    public class ModuleShellViewModel : BindableBase
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Public Properties

        public ISnackbarMessageQueue MessageQueue { get; }

        #endregion

        #region Constructor

        public ModuleShellViewModel(IEventAggregator eventAggregator, ISnackbarMessageQueue messageQueue)
        {
            MessageQueue = messageQueue;

            eventAggregator.GetEvent<ShowSnackbarMessageEvent>().Subscribe(ShowSnackbarMessage);
        }

        #endregion

        #region Private Methods

        private void ShowSnackbarMessage(string message)
        {
            _logger.Debug($"Showing message: {message}");

            MessageQueue.Enqueue(message);
        }

        #endregion
    }
}
