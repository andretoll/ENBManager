using ENBManager.Configuration.Models;
using ENBManager.Infrastructure.Constants;
using System.IO;

namespace ENBManager.Infrastructure.BusinessEntities
{
    public class GameSettings : BaseSettings
    {
        #region Private Members

        private static readonly string PATH = Paths.GetGamesDirectory();
        private const string FILE_NAME = "gamesettings.json";

        #endregion

        #region Settings

        public string InstalledLocation { get; set; } = null;
        public string ActivePreset { get; set; } = null;
        public bool ScreenshotsEnabled { get; set; } = false;

        #endregion

        #region Constructors

        public GameSettings(string directory)
            : base(Path.Combine(PATH, directory, FILE_NAME))
        {
        }

        public GameSettings()
            : base("")
        {

        }

        #endregion
    }
}
