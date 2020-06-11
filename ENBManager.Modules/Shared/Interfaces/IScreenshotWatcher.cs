using System.IO;

namespace ENBManager.Modules.Shared.Interfaces
{
    public interface IScreenshotWatcher
    {
        event FileSystemEventHandler FileCreated;

        /// <summary>
        /// Stops the watcher and unsubscribes from any events.
        /// </summary>
        void Stop();

        /// <summary>
        /// Configures the watcher with directory and filters.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="filters"></param>
        void Configure(string directory, params string[] filters);

        /// <summary>
        /// Starts the watcher and monitors the configured directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="filters"></param>
        void Start();
    }
}
