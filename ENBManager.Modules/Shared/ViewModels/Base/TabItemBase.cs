using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Modules.Shared.Events;
using ENBManager.Modules.Shared.Interfaces;
using NLog;
using Prism.Events;
using Prism.Mvvm;

namespace ENBManager.Modules.Shared.ViewModels.Base
{
    public abstract class TabItemBase : BindableBase, ITabItem
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructor

        public TabItemBase(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<ModuleActivatedEvent>().Subscribe(OnModuleActivated);

            _logger.Debug($"{this.GetType().Name} initialized");
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
