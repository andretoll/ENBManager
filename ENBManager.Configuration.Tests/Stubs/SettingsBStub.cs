using ENBManager.Configuration.Models;
using ENBManager.Infrastructure.Constants;
using System.IO;

namespace ENBManager.Configuration.Tests.Stubs
{
    internal class SettingsBStub : BaseSettings
    {
        private static readonly string PATH = Path.Combine(Paths.GetBaseDirectory(), "appsettings_b//appsettings_b.json");

        public string Text { get; set; }
        public bool Condition { get; set; }

        public SettingsBStub() 
            : base(PATH)
        {
        }
    }
}
