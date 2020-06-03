﻿namespace ENBManager.Infrastructure.BusinessEntities.Dialogs
{
    public class MessageDialog
    {
        #region Public Properties

        public string Message { get; set; }

        #endregion

        #region Constructor

        public MessageDialog(string message)
        {
            Message = message;
        }

        #endregion
    }
}
