using ENBManager.Configuration.Models;

namespace ENBManager.Configuration.Tests.Stubs
{
    public class GameSettingsStub : BaseSettings
    {
        #region Private Members

        private const string FILE_PATH = "test_game_settings//gamesettings.json";

        #endregion

        #region Public Properties

        public bool Condition { get; set; }
        public string Name { get; set; }

        #endregion

        #region Constructor

        public GameSettingsStub()
            : base(FILE_PATH)
        { }

        #endregion
    }
}
