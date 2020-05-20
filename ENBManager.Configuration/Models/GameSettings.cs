using ENBManager.Configuration.Models.Base;
using ENBManager.Infrastructure.Constants;
using System.IO;

namespace ENBManager.Configuration.Models
{
    public class GameSettings : BaseSettings
    {
        #region Private Members

        private const string FILE_NAME = "gamesettings.json";

        #endregion

        #region Settings

        public bool Managed { get; set; } = true;

        #endregion

        #region Constructor

        public GameSettings(string directory)
            : base(Path.Combine(Paths.GAMES_DIRECTORY, directory, FILE_NAME))
        {
        }

        #endregion
    }
}
