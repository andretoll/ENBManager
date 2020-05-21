using ENBManager.Infrastructure.BusinessEntities.Base;

namespace ENBManager.Core.Tests
{
    public class AppSettingsStub : BaseSettings
    {
        #region Private Members

        private const string FILE_PATH = "test_app_settings//appsettings.json"; 

        #endregion

        #region Public Properties

        public bool Condition { get; set; }
        public string Name { get; set; }

        #endregion

        #region Constructor

        public AppSettingsStub()
            : base(FILE_PATH)
        { }

        #endregion
    }
}
