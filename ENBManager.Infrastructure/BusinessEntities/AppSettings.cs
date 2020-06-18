using ENBManager.Configuration.Models;
using ENBManager.Infrastructure.Constants;
using ENBManager.Logging.Enums;
using System.IO;

namespace ENBManager.Infrastructure.BusinessEntities
{
    public class AppSettings : BaseSettings
    {
        #region Private Members

        private static readonly string PATH = Path.Combine(Paths.GetBaseDirectory(), "appsettings.json");

        #endregion

        #region Settings

        public bool Initialized { get; set; } = false;
        public bool OpenLastActiveGame { get; set; } = true;
        public string LastActiveGame { get; set; } = "";
        public bool DarkMode { get; set; } = true;
        public bool DarkModeShortcut { get; set; } = true;
        public string ColorScheme { get; set; } = "Flamingo";
        public LogLevel LogLevel { get; set; } = LogLevel.Information;
        public bool DefaultPresetView { get; set; } = true;
        public bool ManageBinaries { get; set; } = true;
        public bool EnableScreenshotWithoutPreset { get; set; } = true;
        public bool DeleteScreenshotsWhenDeletingPreset { get; set; } = true;
        public bool MinimizeToTray { get; set; } = true;
        public bool RunGameShortcut { get; set; } = false;

        #endregion

        #region Constructor

        public AppSettings()
            : base(PATH)
        {
        }

        #endregion
    }
}
