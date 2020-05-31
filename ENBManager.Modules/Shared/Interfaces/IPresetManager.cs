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

        /// <summary>
        /// Renames a preset in folder and returns the new full path.
        /// </summary>
        /// <param name="preset"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        string RenamePreset(Preset preset, string newName);

        /// <summary>
        /// Deletes a preset in folder.
        /// </summary>
        /// <param name="preset"></param>
        void DeletePreset(Preset preset);
    }
}
