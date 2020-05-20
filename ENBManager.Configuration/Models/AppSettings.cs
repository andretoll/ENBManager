using ENBManager.Configuration.Models.Base;

namespace ENBManager.Configuration.Models
{
    public class AppSettings : BaseSettings
    {
        #region Private Members

        private const string FILE_NAME = "appsettings.json";

        #endregion

        #region Settings

        public bool Initialized { get; set; } = false;
        public bool OpenLastActiveGame { get; set; } = true;
        public string LastActiveGame { get; set; } = "";
        public bool DarkMode { get; set; } = false;

        #endregion

        #region Constructor

        public AppSettings()
            : base(FILE_NAME)
        {
        }

        #endregion
    }
}
