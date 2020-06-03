namespace ENBManager.Infrastructure.BusinessEntities.Dialogs
{
    public class ConfirmDialog
    {
        #region Public Properties

        public string Message { get; set; }

        #endregion

        #region Constructor

        public ConfirmDialog(string message)
        {
            Message = message;
        }

        #endregion
    }
}
