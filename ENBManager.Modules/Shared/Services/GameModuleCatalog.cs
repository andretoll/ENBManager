using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Modules.Shared.Interfaces;
using Prism.Ioc;
using System;
using System.Collections.Generic;

namespace ENBManager.Modules.Shared.Services
{
    public class GameModuleCatalog : IGameModuleCatalog
    {
        #region Private Members

        private readonly List<GameModule> _gameModules;

        #endregion

        #region Constructor

        public GameModuleCatalog()
        {
            _gameModules = new List<GameModule>();
        } 

        #endregion

        #region IGameModuleCatalog Implementation

        public IEnumerable<GameModule> GameModules => _gameModules;

        public void Register<T>(IContainerProvider container) where T : GameModule
        {
            _gameModules.Add((GameModule)Activator.CreateInstance(typeof(T), container));
        }

        #endregion
    }
}
