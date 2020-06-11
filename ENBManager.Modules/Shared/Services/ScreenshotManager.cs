using ENBManager.Modules.Shared.Interfaces;
using NLog;
using System.IO;
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

        public void SaveScreenshot(string directory, string path)
        {
            _logger.Debug(nameof(SaveScreenshot));

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            Thread.Sleep(SCREENSHOT_TIMEOUT);
            File.Move(path, Path.Combine(directory, Path.GetFileName(path)), true);
        }  

        #endregion
    }
}
