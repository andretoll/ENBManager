using ENBManager.Infrastructure.BusinessEntities;
using Prism.Ioc;
using System.Collections.Generic;

namespace ENBManager.Modules.Shared.Interfaces
{
    public interface IGameModuleCatalog
    {
        /// <summary>
        /// Returns the registered game modules.
        /// </summary>
        IEnumerable<GameModule> GameModules { get; }

        /// <summary>
        /// Adds a module to the catalog. Accepts a dependency container as argument.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        void AddModule<T>(IContainerProvider container) where T : GameModule;
    }
}
