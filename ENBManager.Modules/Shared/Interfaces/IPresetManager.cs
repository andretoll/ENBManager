using ENBManager.Modules.Shared.Models;
using System.Collections.Generic;

namespace ENBManager.Modules.Shared.Interfaces
{
    public interface IPresetManager
    {
        /// <summary>
        /// Gets the presets located in folder.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Preset> GetPresets(string path);
    }
}
