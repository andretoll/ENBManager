using ENBManager.Modules.Shared.Interfaces;
using NLog;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace ENBManager.Modules.Shared.Services
{
    public class ScreenshotManager : IScreenshotManager
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private const int SCREENSHOT_TIMEOUT = 3000;

        #endregion

        #region IScreenshotManager Implementation

        public List<string> GetScreenshots(string directory, bool includeSubdirectories = false)
        {
            _logger.Debug("Getting screenshots");

            if (!Directory.Exists(directory))
                return new List<string>();

            return Directory.GetFiles(directory, "*", includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToList();
        }

        public void SaveScreenshot(string directory, string path)
        {
            _logger.Debug($"Saving screenshot {path}");

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            Thread.Sleep(SCREENSHOT_TIMEOUT);
            File.Move(path, Path.Combine(directory, Path.GetFileName(path)), true);
        }

        public void RenameScreenshotDirectory(string directory, string newName)
        {
            _logger.Debug("Renaming screenshot directory");

            string destination = Path.Combine(new DirectoryInfo(directory).Parent.FullName, newName);

            if (directory.Equals(destination))
                return;

            Directory.Move(directory, destination);
        }

        public void DeleteScreenshotDirectory(string directory)
        {
            _logger.Debug("Deleting screenshot directory");

            if (Directory.Exists(directory))
                Directory.Delete(directory, true);
        }

        #endregion
    }
}
