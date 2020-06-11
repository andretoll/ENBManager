using ENBManager.Modules.Shared.Interfaces;
using NLog;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;

namespace ENBManager.Modules.Shared.Services
{
    public class ScreenshotWatcher : IScreenshotWatcher
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly FileSystemWatcher _watcher;

        #endregion

        #region Constructor

        public ScreenshotWatcher()
        {
            _watcher = new FileSystemWatcher();

            _logger.Debug($"{nameof(ScreenshotWatcher)} initialized");
        }

        #endregion

        #region Events

        public event FileSystemEventHandler FileCreated;

        #endregion

        #region Public Methods

        public void Stop()
        {
            _watcher.Created -= FileCreated;

            _watcher.EnableRaisingEvents = false;
            
            _logger.Info("ScreenshotWatcher stopped");
        }

        public void Configure(string directory, params string[] filters)
        {
            _watcher.Path = directory;
            _watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.LastWrite;
            _watcher.Filters.AddRange(filters);

            _logger.Info("ScreenshotWatcher configured");
        }

        public void Start()
        {
            if (string.IsNullOrEmpty(_watcher.Path) || _watcher.Filters.Count == 0)
                throw new ConfigurationErrorsException("The watcher is not configured.");

            _watcher.Created += FileCreated;

            _watcher.EnableRaisingEvents = true;

            _logger.Info("ScreenshotWatcher started");
        }

        #endregion


    }
}
