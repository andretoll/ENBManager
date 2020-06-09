using ENBManager.Infrastructure.Constants;
using ENBManager.Infrastructure.Interfaces;
using Microsoft.Win32;
using NLog;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ENBManager.Infrastructure.Services
{
    public class GameService : IGameService
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region IFileService Implementation

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

        #endregion
    }
}
