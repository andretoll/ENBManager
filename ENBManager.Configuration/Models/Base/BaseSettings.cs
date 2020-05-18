using ENBManager.Infrastructure.Constants;
using System.IO;

namespace ENBManager.Configuration.Models.Base
{
    public abstract class BaseSettings
    {
        #region Private Members

        private string _path;

        #endregion

        #region Constructor

        public BaseSettings(string filePath)
        {
            _path = Path.Combine(Paths.GetBaseDirectory(), filePath);
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
