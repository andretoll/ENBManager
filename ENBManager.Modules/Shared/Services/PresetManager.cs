using ENBManager.Infrastructure.Exceptions;
using ENBManager.Modules.Shared.Interfaces;
using ENBManager.Modules.Shared.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ENBManager.Modules.Shared.Services
{
    public class PresetManager : IPresetManager
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region IPresetManager Implementation

        public IEnumerable<Preset> GetPresets(string path)
        {
            _logger.Debug("Getting presets");

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

        public Task<Preset> GetPresetAsync(string path, string preset)
        {
            _logger.Debug("Getting preset");

            var presets = GetPresets(path);

            return Task.FromResult(presets.Single(x => x.Name == preset));
        }

        public string RenamePreset(Preset preset, string newName)
        {
            _logger.Debug($"Renaming preset {preset.Name} to {newName}");

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

            // Make sure no other preset with new name exists
            if (Directory.Exists(newDirectory))
                throw new IdenticalNameException("Rename preset failed. Preset with name already exists.");

            oldDirectory.MoveTo(newDirectory);

            return newDirectory;
        }

        public void DeletePreset(Preset preset)
        {
            _logger.Debug("Deleting preset");

            // Make sure directory exists
            if (!Directory.Exists(preset.FullPath))
                throw new DirectoryNotFoundException($"Preset with name {preset.Name} does not exist.");

            Directory.Delete(preset.FullPath, true);
        }

        public async Task ActivatePresetAsync(string targetDir, Preset preset)
        {
            _logger.Debug("Activating preset");

            // Create all directories
            foreach (var dir in Directory.GetDirectories(preset.FullPath, "*", SearchOption.AllDirectories))
            {
                if (!Directory.Exists(dir.Replace(preset.FullPath, targetDir)))
                    Directory.CreateDirectory(dir.Replace(preset.FullPath, targetDir));
            }

            // Copy all files
            foreach (var file in Directory.GetFiles(preset.FullPath, "*", SearchOption.AllDirectories))
            {
                File.Copy(file, file.Replace(preset.FullPath, targetDir), true);
            }

            await Task.Delay(500);
        }

        public async Task DeactivatePresetAsync(string targetDir, Preset preset)
        {
            _logger.Debug("Deactivating preset");

            // Delete all files
            foreach (var file in Directory.GetFiles(targetDir, "*", SearchOption.AllDirectories))
            {
                if (preset.Files.Contains(file.Replace(targetDir, preset.FullPath)))
                {
                    File.Delete(file);
                }
            }

            // Delete all empty directories
            var sourceDirs = Directory.GetDirectories(preset.FullPath, "*", SearchOption.AllDirectories);
            foreach (var dir in Directory.GetDirectories(targetDir))
            {
                if (sourceDirs.Contains(dir.Replace(targetDir, preset.FullPath)) && 
                    Directory.GetFiles(dir, "*", SearchOption.AllDirectories).Length == 0)
                {
                    Directory.Delete(dir, true);
                }
            }
            
            await Task.Delay(500);
        }

        public Preset CreateExistingPreset(string targetDir)
        {
            _logger.Debug("Creating existing preset");

            var enbFiles = Directory.GetFiles(targetDir, "*enb*.*", SearchOption.TopDirectoryOnly).ToList();

            var enbDirs = Directory.GetDirectories(targetDir, "*enb*", SearchOption.TopDirectoryOnly);

            foreach (var dir in enbDirs)
            {
                // If directory is empty, add it
                if (Directory.GetFiles(dir).Length == 0)
                    enbFiles.Add(dir);

                var files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    enbFiles.Add(file);
                }
            }

            var preset = new Preset()
            {
                Files = enbFiles
            };

            return preset;
        }

        public async Task SaveCurrentPresetAsync(string targetDir, string sourceDir, Preset preset)
        {
            _logger.Debug("Saving current preset");

            var presetsDir = targetDir;

            if (Directory.Exists(Path.Combine(presetsDir, preset.Name)))
            {
                throw new IdenticalNameException("Preset with this name already exists.");
            }

            // Create root folder
            preset.FullPath = Directory.CreateDirectory(Path.Combine(presetsDir, preset.Name)).FullName;

            foreach (var file in preset.Files)
            {
                // If directory is empty, create it
                if (File.GetAttributes(file).HasFlag(FileAttributes.Directory) && Directory.GetFiles(file).Length == 0)
                {
                    Directory.CreateDirectory(file.Replace(sourceDir, preset.FullPath));
                    continue;
                }

                var parentDir = Directory.GetParent(file.Replace(sourceDir, preset.FullPath));

                // Create subfolder
                if (!Directory.Exists(parentDir.FullName))
                {
                    Directory.CreateDirectory(parentDir.FullName);
                }

                // Copy file
                File.Copy(file, file.Replace(sourceDir, preset.FullPath));
            }

            await Task.Delay(500);
        }

        public async Task SaveNewPresetAsync(string targetDir, Preset preset)
        {
            _logger.Debug("Saving new preset");

            string originRoot = preset.FullPath;

            if (Directory.Exists(Path.Combine(targetDir, preset.Name)))
            {
                throw new IdenticalNameException("Preset with this name already exists.");
            }

            // Create root folder
            preset.FullPath = Directory.CreateDirectory(Path.Combine(targetDir, preset.Name)).FullName;

            foreach (var file in preset.Files)
            {
                var parentDir = Directory.GetParent(file.Replace(originRoot, preset.FullPath));

                // Create subfolder
                if (!Directory.Exists(parentDir.FullName))
                {
                    Directory.CreateDirectory(parentDir.FullName);
                }

                // Copy file
                if (File.Exists(file))
                    File.Copy(file, file.Replace(originRoot, preset.FullPath));
                // Create empty folder
                else if (Directory.Exists(file))
                    Directory.CreateDirectory(file.Replace(originRoot, preset.FullPath));
            }

            await Task.Delay(500);
        }

        public Task<bool> ValidatePresetAsync(string targetDir, Preset preset)
        {
            _logger.Debug("Validating preset");

            bool identical = true;

            foreach (var file in preset.Files)
            {
                string file1 = file;
                string file2 = file.Replace(preset.FullPath, targetDir);

                string hash1, hash2;

                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(file1))
                    {
                        hash1 = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                    }
                    using (var stream = File.OpenRead(file2))
                    {
                        hash2 = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                    }
                }

                if (hash1 != hash2)
                    identical = false;
            }

            _logger.Debug($"Identical - {identical}");

            return Task.FromResult(identical);
        }

        public Task UpdatePresetFilesAsync(string targetDir, Preset preset)
        {
            _logger.Debug("Updating preset files");

            foreach (var item in preset.Files)
            {
                string source = item.Replace(preset.FullPath, targetDir);

                File.Delete(item);

                if (File.Exists(source))
                    File.Copy(source, source.Replace(targetDir, preset.FullPath));
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
