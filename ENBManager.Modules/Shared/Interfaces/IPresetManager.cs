using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Modules.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        /// Gets a specific preset in folder.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<Preset> GetPreset(GameModule gameModule, string preset);

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

        /// <summary>
        /// Activates a preset.
        /// </summary>
        /// <param name="preset"></param>
        Task ActivatePresetAsync(GameModule gameModule, Preset preset);

        /// <summary>
        /// Deactivates a preset.
        /// </summary>
        /// <param name="preset"></param>
        Task DeactivatePresetAsync(GameModule gameModule, Preset preset);

        /// <summary>
        /// Creates a preset based on current ENB files.
        /// </summary>
        /// <param name="gameModule"></param>
        /// <returns></returns>
        Preset CreateExistingPreset(GameModule gameModule);

        /// <summary>
        /// Saves the current preset to folder.
        /// </summary>
        /// <param name="gameModule"></param>
        /// <param name="preset"></param>
        /// <returns></returns>
        Task SaveCurrentPresetAsync(GameModule gameModule, Preset preset);

        /// <summary>
        /// Saves a new preset to folder.
        /// </summary>
        /// <param name="gameModule"></param>
        /// <param name="preset"></param>
        /// <returns></returns>
        Task SaveNewPresetAsync(GameModule gameModule, Preset preset);

        /// <summary>
        /// Validates an active preset.
        /// </summary>
        /// <param name="gameModule"></param>
        /// <param name="preset"></param>
        /// <returns></returns>
        Task<bool> ValidatePreset(GameModule gameModule, Preset preset);

        /// <summary>
        /// Updates the provided preset.
        /// </summary>
        /// <param name="gameModule"></param>
        /// <param name="preset"></param>
        Task UpdatePresetFiles(GameModule gameModule, Preset preset);
    }
}
