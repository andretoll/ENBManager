using System.Collections.Generic;

namespace ENBManager.Modules.Shared.Interfaces
{
    public interface IScreenshotManager
    {
        /// <summary>
        /// Gets the screenshots in the provided directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        List<string> GetScreenshots(string directory);

        /// <summary>
        /// Saves a screenshot to the provided directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="path"></param>
        void SaveScreenshot(string directory, string path);

        /// <summary>
        /// Renames a screenshot directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="newName"></param>
        void RenameScreenshotDirectory(string directory, string newName);

        /// <summary>
        /// Deletes a screenshot directory.
        /// </summary>
        /// <param name="directory"></param>
        void DeleteScreenshotDirectory(string directory);
    }
}
