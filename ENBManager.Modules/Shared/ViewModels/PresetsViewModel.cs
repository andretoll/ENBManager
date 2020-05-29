using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Localization.Strings;
using ENBManager.Modules.Shared.ViewModels.Base;
using Prism.Events;

namespace ENBManager.Modules.Shared.ViewModels
{
    public class PresetsViewModel : TabItemBase
    {
        public PresetsViewModel(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {

        }

        public override string Name => Strings.PRESETS;

        protected override void OnModuleActivated(GameModule game)
        {
        }
    }
}
