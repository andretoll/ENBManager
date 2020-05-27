using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Localization.Strings;
using ENBManager.Modules.Shared.Interfaces;

namespace ENBManager.Modules.Shared.ViewModels
{
    public class PresetsViewModel : ITabItem
    {
        public PresetsViewModel(GameModule game)
        {

        }

        public string Name => Strings.PRESETS;
    }
}
