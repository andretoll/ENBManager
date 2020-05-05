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

        private string _directoryPath;
        private string _filePath;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor that accepts a path to the settings file, excluding the base directory.
        /// </summary>
        /// <param name="filePath"></param>
        public BaseSettings(string filePath)
        {
            _directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), BASE_DIRECTORY);
            _filePath = filePath;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the complete path to the settings file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string GetFilePath()
        {
            return Path.Combine(_directoryPath, _filePath);
        }

        #endregion
    }
}
