using ENBManager.Configuration.Models.Base;
using System.IO;

namespace ENBManager.Configuration.Models
{
    public class GameSettings : BaseSettings
    {
        #region Private Members

        private new const string BASE_DIRECTORY = "Games";
        private const string FILE_NAME = "gamesettings.json";

        #endregion

        #region Settings



        #endregion

        #region Constructor

        public GameSettings(string directory)
            : base(Path.Combine(BASE_DIRECTORY, directory, FILE_NAME))
        {
            // Set factory values
        }

        #endregion
    }
}
