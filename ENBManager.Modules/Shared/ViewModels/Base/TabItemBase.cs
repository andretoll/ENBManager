using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Modules.Shared.Events;
using ENBManager.Modules.Shared.Interfaces;
using Prism.Events;

namespace ENBManager.Modules.Shared.ViewModels.Base
{
    public abstract class TabItemBase : ITabItem
    {
        #region Constructor

        public TabItemBase(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<ModuleActivatedEvent>().Subscribe(OnModuleActivated);
        } 

        #endregion

        #region ITabItem Implementation

        public abstract string Name { get; } 

        #endregion

        #region Abstract Methods

        protected abstract void OnModuleActivated(GameModule game); 

        #endregion
    }
}
