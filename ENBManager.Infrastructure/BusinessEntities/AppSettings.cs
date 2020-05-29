using ENBManager.Configuration.Models;
using ENBManager.Infrastructure.Constants;
using ENBManager.Logging.Enums;
using System.IO;

namespace ENBManager.Infrastructure.BusinessEntities
{
    public class AppSettings : BaseSettings
    {
        #region Private Members

        private static string FILE_NAME = Path.Combine(Paths.GetBaseDirectory(), "appsettings.json");

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
