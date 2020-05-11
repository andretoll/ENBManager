using ENBManager.Core.BusinessEntities;
using System.Collections.Generic;

namespace ENBManager.Core.Interfaces
{
    public interface IGameRegistry
    {
        /// <summary>
        /// Returns the currently supported games.
        /// </summary>
        /// <returns></returns>
        IEnumerable<InstalledGame> GetSupportedGames(); 
    }
}
