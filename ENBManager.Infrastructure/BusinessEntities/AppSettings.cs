using ENBManager.Infrastructure.BusinessEntities.Base;

namespace ENBManager.Infrastructure.BusinessEntities
{
    public class AppSettings : BaseSettings
    {
        private const string FILE_PATH = "appsettings.json";

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
