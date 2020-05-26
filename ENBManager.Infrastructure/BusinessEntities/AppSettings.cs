using ENBManager.Infrastructure.BusinessEntities.Base;
using ENBManager.Logging.Enums;

namespace ENBManager.Infrastructure.BusinessEntities
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
        public bool DarkModeShortcut { get; set; } = false;
        public string ColorScheme { get; set; } = "Fire";
        public LogLevel LogLevel { get; set; } = LogLevel.Information;

        #endregion

        #region Constructor

        public AppSettings()
            : base(FILE_NAME)
        {
        }

        #endregion
    }
}
