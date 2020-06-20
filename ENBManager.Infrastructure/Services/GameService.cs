using ENBManager.Infrastructure.Constants;
using ENBManager.Infrastructure.Enums;
using ENBManager.Infrastructure.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ENBManager.Infrastructure.Services
{
    public class GameService : IGameService
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region IGameService Implementation

        public void DeleteGameDirectory(string module)
        {
            _logger.Debug($"Deleting directory '{module}'");

            Directory.Delete(Path.Combine(Paths.GetGamesDirectory(), module), true);
        }

        public string[] GetGameDirectories()
        {
            _logger.Debug("Getting game directories");

            return Directory.GetDirectories(Paths.GetGamesDirectory());
        }

        public bool VerifyBinaries(string directoryPath, string[] binaries, out string[] missingBinaries)
        {
            _logger.Debug("Verifying binaries and returns missing files");

            List<string> missingFiles = new List<string>();

            foreach (var binary in binaries)
            {
                if (!File.Exists(Path.Combine(directoryPath, binary)))
                {
                    missingFiles.Add(binary);
                }
            }

            missingBinaries = missingFiles.ToArray();

            return missingBinaries.Length == 0;
        }

        public bool VerifyBinaries(string directoryPath, string[] binaries)
        {
            _logger.Debug("Verifying binaries");

            foreach (var binary in binaries)
            {
                if (!File.Exists(Path.Combine(directoryPath, binary)))
                {
                    return false;
                }
            }

            return true;
        }

        public void CopyBinaries(string source, string target, string[] binaries)
        {
            _logger.Debug($"Copying binaries from {source} to {target}");

            foreach (var binary in binaries)
            {
                if (!Directory.Exists(target))
                    Directory.CreateDirectory(target);

                File.Copy(Path.Combine(source, binary), Path.Combine(target, binary), true);
            }
        }

        public void DeleteBinaries(string target, string[] binaries)
        {
            _logger.Debug("Deleting binaries");

            foreach (var binary in binaries)
            {
                if (File.Exists(Path.Combine(target, binary)))
                    File.Delete(Path.Combine(target, binary));
            }
        }

        public VersionMismatch VerifyBinariesVersion(string source, string target, string[] binaries)
        {
            _logger.Debug("Verifying binaries version");

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
                            _logger.Debug(VersionMismatch.Matching.ToString());
                            return VersionMismatch.Matching;
                        case 1:
                            _logger.Debug(VersionMismatch.Above.ToString());
                            return VersionMismatch.Above;
                        case -1:
                            _logger.Debug(VersionMismatch.Below.ToString());
                            return VersionMismatch.Below;
                    }
                }
            }

            return VersionMismatch.Matching;
        }

        public string[] AppendBinaryVersions(string target, string[] binaries)
        {
            _logger.Debug("Getting binaries version");

            for (int i = 0; i < binaries.Length; i++)
            {
                string info = FileVersionInfo.GetVersionInfo(Path.Combine(target, binaries[i])).ProductVersion.Replace(',', '.');

                binaries[i] += " (" + info + ")";
            }

            return binaries;
        }

        #endregion
    }
}
