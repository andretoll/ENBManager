using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Modules.Shared.Interfaces;
using ENBManager.Modules.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
                var preset = new Preset
                {
                    FullPath = dir,
                    Name = Path.GetFileName(dir),
                    Files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories)
                };

                presets.Add(preset);
            }

            return presets;
        }

        public string RenamePreset(Preset preset, string newName)
        {
            var oldDirectory = new DirectoryInfo(preset.FullPath);

            // Make sure new name has a value
            if (string.IsNullOrEmpty(newName))
                throw new ArgumentNullException($"New name is null or empty.");

            // Make sure directory exists
            if (!Directory.Exists(preset.FullPath))
                throw new DirectoryNotFoundException($"Preset with name {preset.Name} does not exist.");

            // Make sure new name is a different name
            if (preset.Name == newName)
                throw new ArgumentException("New name is identical to old name.");

            string newDirectory = Path.Combine(oldDirectory.Parent.FullName, newName);

            oldDirectory.MoveTo(newDirectory);

            return newDirectory;
        }

        public void DeletePreset(Preset preset)
        {
            // Make sure directory exists
            if (!Directory.Exists(preset.FullPath))
                throw new DirectoryNotFoundException($"Preset with name {preset.Name} does not exist.");

            Directory.Delete(preset.FullPath, true);
        }

        public async Task ActivatePreset(GameModule gameModule, Preset preset)
        {
            // Create all directories
            foreach (var dir in Directory.GetDirectories(preset.FullPath, "*", SearchOption.AllDirectories))
            {
                if (!Directory.Exists(dir.Replace(preset.FullPath, gameModule.InstalledLocation)))
                    Directory.CreateDirectory(dir.Replace(preset.FullPath, gameModule.InstalledLocation));
            }

            // Copy all files
            foreach (var file in Directory.GetFiles(preset.FullPath, "*", SearchOption.AllDirectories))
            {
                File.Copy(file, file.Replace(preset.FullPath, gameModule.InstalledLocation), true);
            }

            await Task.Delay(500);
        }

        public async Task DeactivatePreset(GameModule gameModule, Preset preset)
        {
            // Delete all files
            foreach (var file in Directory.GetFiles(gameModule.InstalledLocation, "*", SearchOption.AllDirectories))
            {
                if (preset.Files.Contains(file.Replace(gameModule.InstalledLocation, preset.FullPath)))
                {
                    File.Delete(file);
                }
            }

            // Delete all empty directories
            var sourceDirs = Directory.GetDirectories(preset.FullPath, "*", SearchOption.AllDirectories);
            foreach (var dir in Directory.GetDirectories(gameModule.InstalledLocation))
            {
                if (sourceDirs.Contains(dir.Replace(gameModule.InstalledLocation, preset.FullPath)) && 
                    Directory.GetFiles(dir, "*", SearchOption.AllDirectories).Length == 0)
                {
                    Directory.Delete(dir, true);
                }
            }

            await Task.Delay(500);
        }

        #endregion
    }
}
