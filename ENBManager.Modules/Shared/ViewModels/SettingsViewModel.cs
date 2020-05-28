using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Localization.Strings;
using ENBManager.Modules.Shared.Interfaces;

namespace ENBManager.Modules.Shared.ViewModels
{
    public class SettingsViewModel : ITabItem
    {
        public SettingsViewModel(GameModule game)
        {

        }

        public string Name => Strings.SETTINGS;
    }
}
