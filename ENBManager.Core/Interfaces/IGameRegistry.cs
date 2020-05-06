using ENBManager.Core.BusinessEntities.Base;
using System.Collections.Generic;

namespace ENBManager.Core.Interfaces
{
    public interface IGameRegistry
    {
        /// <summary>
        /// Returns the currently supported games.
        /// </summary>
        /// <returns></returns>
        IEnumerable<GameBase> GetSupportedGames(); 
    }
}
