namespace ENBManager.Configuration.Models
{
    public abstract class BaseSettings
    {
        #region Private Members

        private string _fullPath;

        #endregion

        #region Constructor

        public BaseSettings(string fullPath)
        {
            _fullPath = fullPath;
        }

        #endregion

        #region Public Methods

        public string GetFullPath()
        {
            return _fullPath;
        }

        public void SetFullPath(string fullPath)
        {
            _fullPath = fullPath;
        }

        #endregion
    }
}
