using ENBManager.Infrastructure.BusinessEntities.Base;

namespace ENBManager.Configuration.Tests.Stubs
{
    public class AppSettingsStub : BaseSettings
    {
        private const string FILE_PATH = "test//appsettings.json";

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
