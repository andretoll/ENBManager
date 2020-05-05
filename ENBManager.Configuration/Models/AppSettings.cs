using ENBManager.Configuration.Models.Base;

namespace ENBManager.Configuration.Models
{
    public class AppSettings : BaseSettings
    {
        #region Private Members

        private const string FILE_PATH = "appsettings.json"; 

        #endregion

        #region Public Properties

        public bool Initialized { get; }

        #endregion

        #region Constructor

        public AppSettings()
            : base(FILE_PATH)
        {
            // Set factory values
            Initialized = false;
        }

        #endregion
    }
}
