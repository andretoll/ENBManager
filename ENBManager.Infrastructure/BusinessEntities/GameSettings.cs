using ENBManager.Infrastructure.BusinessEntities.Base;
using ENBManager.Infrastructure.Constants;
using System.IO;

namespace ENBManager.Infrastructure.BusinessEntities
{
    public class GameSettings : BaseSettings
    {
        #region Private Members

        private const string FILE_NAME = "gamesettings.json";

        #endregion

        #region Settings

        public string InstalledLocation { get; set; }

        #endregion

        #region Constructor

        public GameSettings(string directory)
            : base(Path.Combine(Paths.GAMES_DIRECTORY, directory, FILE_NAME))
        {
        }

        public GameSettings()
            : base("")
        {

        }

        #endregion
    }
}
