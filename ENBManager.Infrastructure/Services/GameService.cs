using ENBManager.Infrastructure.Constants;
using ENBManager.Infrastructure.Enums;
using ENBManager.Infrastructure.Interfaces;
using Microsoft.Win32;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ENBManager.Infrastructure.Services
{
    public class GameService : IGameService
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region IGameService Implementation

        public string BrowseGameExecutable(string fileName)
        {
            _logger.Info($"Browsing for file '{fileName}'");

            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = $"{fileName.Split('.').First()} ({fileName}) | {fileName}",
                CheckFileExists = true
            };

            bool? cancelled;
            do
            {
                cancelled = !dialog.ShowDialog();
            }
            while (!cancelled.Value && !string.IsNullOrEmpty(dialog.FileName) &&
                Path.GetFileName(dialog.FileName) != fileName);

            if (cancelled.Value)
                return null;

            return dialog.FileName;
        }

        public void DeleteGameDirectory(string directoryName)
        {
            _logger.Info($"Deleting directory '{directoryName}'");

            Directory.Delete(Path.Combine(Paths.GetGamesDirectory(), directoryName), true);
        }

        public string[] GetGameDirectories()
        {
            _logger.Debug(nameof(GetGameDirectories));

            return Directory.GetDirectories(Paths.GetGamesDirectory());
        }

        public string[] VerifyBinaries(string directoryPath, string[] files)
        {
            _logger.Debug(nameof(VerifyBinaries));

            List<string> missingFiles = new List<string>();

            foreach (var file in files)
            {
                if (!File.Exists(Path.Combine(directoryPath, file)))
                {
                    missingFiles.Add(file);
                }
            }

            return missingFiles.ToArray();
        }

        public bool VerifyBinariesBackup(string directoryPath, string[] binaries)
        {
            _logger.Debug(nameof(VerifyBinariesBackup));

            foreach (var binary in binaries)
            {
                if (!File.Exists(Path.Combine(directoryPath, binary)))
                    return false;
            }

            return true;
        }

        public void CopyBinaries(string source, string target, string[] binaries)
        {
            _logger.Debug(nameof(CopyBinaries));

            foreach (var binary in binaries)
            {
                if (!Directory.Exists(target))
                    Directory.CreateDirectory(target);

                File.Copy(Path.Combine(source, binary), Path.Combine(target, binary), true);
            }
        }

        public void DeleteBinaries(string target, string[] binaries)
        {
            _logger.Debug(nameof(DeleteBinaries));

            foreach (var binary in binaries)
            {
                if (File.Exists(Path.Combine(target, binary)))
                    File.Delete(Path.Combine(target, binary));
            }
        }

        public VersionMismatch VerifyBinariesVersion(string source, string target, string[] binaries)
        {
            _logger.Debug(nameof(VerifyBinariesVersion));

            foreach (var binary in binaries)
            {
                string sourcePath = Path.Combine(source, binary);
                string targetPath = Path.Combine(target, binary);

                if (File.Exists(sourcePath) && File.Exists(targetPath))
                {
                    var v1 = FileVersionInfo.GetVersionInfo(sourcePath).ProductVersion.Replace(',', '.');
                    var sourceVersion = new Version(v1);
                    var v2 = FileVersionInfo.GetVersionInfo(targetPath).ProductVersion.Replace(',', '.');
                    var targetVersion = new Version(v2);

                    var result = sourceVersion.CompareTo(targetVersion);

                    switch (result)
                    {
                        case 0:
                            return VersionMismatch.Matching;
                        case 1:
                            return VersionMismatch.Above;
                        case -1:
                            return VersionMismatch.Below;
                    }
                }
            }

            return VersionMismatch.Matching;
        }

        #endregion
    }
}
