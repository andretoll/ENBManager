namespace ENBManager.Configuration.Models
{
    public abstract class BaseSettings
    {
        #region Private Members

        private string _path;

        #endregion

        #region Constructor

        public BaseSettings(string filePath)
        {
            _path = filePath;
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
