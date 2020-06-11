namespace ENBManager.Modules.Shared.Interfaces
{
    public interface IScreenshotManager
    {
        /// <summary>
        /// Saves a screenshot to the provided directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="path"></param>
        void SaveScreenshot(string directory, string path);
    }
}
