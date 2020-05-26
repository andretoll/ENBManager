using ENBManager.Core.Interfaces;
using ENBManager.Infrastructure.Constants;
using Microsoft.Win32;
using NLog;
using System.IO;
using System.Linq;

namespace ENBManager.Core.Services
{
    public class FileService : IFileService
    {
        #region Private Members

        private static Logger _logger = LogManager.GetCurrentClassLogger();

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

            do
            {
                dialog.ShowDialog();
            }
            while (!string.IsNullOrEmpty(dialog.FileName) &&
                Path.GetFileName(dialog.FileName) != fileName);

            return dialog.FileName;
        }

        public void DeleteGameDirectory(string directoryName)
        {
            _logger.Info($"Deleting directory '{directoryName}'");

            Directory.Delete(Path.Combine(Paths.GetBaseDirectory(), Paths.GAMES_DIRECTORY, directoryName), true);
        }

        public string[] GetGameDirectories()
        {
            _logger.Debug(nameof(GetGameDirectories));

            var baseDirectory = Path.Combine(Paths.GetBaseDirectory(), Paths.GAMES_DIRECTORY);

            var directories = Directory.GetDirectories(baseDirectory);

            return directories;
        }

        #endregion
    }
}
