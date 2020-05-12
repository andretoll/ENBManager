using System;
using System.IO;

namespace ENBManager.Configuration.Models.Base
{
    public abstract class BaseSettings
    {
        #region Protected Constants

        protected const string BASE_DIRECTORY = "ENBManager";

        #endregion

        #region Private Members

        private string _path;

        #endregion

        #region Constructor

        public BaseSettings(string filePath)
        {
            _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), BASE_DIRECTORY, filePath);
        }

        #endregion

        #region Public Methods

        public string GetFilePath()
        {
            return _path;
        }

        #endregion
    }
}
