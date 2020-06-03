using ENBManager.Infrastructure.BusinessEntities.Dialogs.Base;

namespace ENBManager.Infrastructure.BusinessEntities.Dialogs
{
    public class MessageDialog : BaseDialog
    {
        #region Constructor

        public MessageDialog(string message)
            : base (message) { }

        #endregion
    }
}
