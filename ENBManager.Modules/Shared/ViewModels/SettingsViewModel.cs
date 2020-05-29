using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Localization.Strings;
using ENBManager.Modules.Shared.ViewModels.Base;
using Prism.Events;

namespace ENBManager.Modules.Shared.ViewModels
{
    public class SettingsViewModel : TabItemBase
    {
        public SettingsViewModel(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
        }

        public override string Name => Strings.SETTINGS;

        protected override void OnModuleActivated(GameModule game)
        {
        }
    }
}
