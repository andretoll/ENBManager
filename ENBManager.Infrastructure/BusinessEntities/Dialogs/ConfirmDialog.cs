using ENBManager.Infrastructure.BusinessEntities.Dialogs.Base;

namespace ENBManager.Infrastructure.BusinessEntities.Dialogs
{
    public class ConfirmDialog : BaseDialog
    {
        #region Constructor

        public ConfirmDialog(string message)
            : base(message) { }

        #endregion
    }
}
