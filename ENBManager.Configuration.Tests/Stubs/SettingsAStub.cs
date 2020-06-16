using ENBManager.Configuration.Models;
using ENBManager.Infrastructure.Constants;
using System.IO;

namespace ENBManager.Configuration.Tests.Stubs
{
    internal class SettingsAStub : BaseSettings
    {
        private static readonly string PATH = Path.Combine(Paths.GetBaseDirectory(), "appsettings_a//appsettings_a.json");

        public string Text { get; set; }
        public bool Condition { get; set; }

        public SettingsAStub() 
            : base(PATH)
        {
        }
    }
}
