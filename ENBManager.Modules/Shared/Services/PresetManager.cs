using ENBManager.Modules.Shared.Interfaces;
using ENBManager.Modules.Shared.Models;
using System.Collections.Generic;
using System.IO;

namespace ENBManager.Modules.Shared.Services
{
    public class PresetManager : IPresetManager
    {
        #region IPresetManager Implementation

        public IEnumerable<Preset> GetPresets(string path)
        {
            var presets = new List<Preset>();

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var dirs = Directory.GetDirectories(path);

            foreach (var dir in dirs)
            {
                var preset = new Preset();
                preset.Name = Path.GetFileName(dir);
                preset.Files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);

                presets.Add(preset);
            }

            return presets;
        } 

        #endregion
    }
}
