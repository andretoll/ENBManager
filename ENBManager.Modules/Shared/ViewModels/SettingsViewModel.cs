using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Modules.Shared.Interfaces;

namespace ENBManager.Modules.Shared.ViewModels
{
    public class SettingsViewModel : ITabItem
    {
        public SettingsViewModel(InstalledGame game)
        {

        }

        public string Name => ENBManager.Localization.Strings.Strings.SETTINGS;
    }
}
