namespace ENBManager.Core.Interfaces
{
    public interface IGameService
    {
        /// <summary>
        /// Prompts user to browse the computer for a specific file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns></returns>
        string BrowseGameExecutable(string fileName);

        /// <summary>
        /// Retrieves the managed game defined by existing folders on disk.
        /// </summary>
        /// <returns></returns>
        string[] GetGameDirectories();

        /// <summary>
        /// Deletes a game directory from disk.
        /// </summary>
        void DeleteGameDirectory(string directoryName);
    }
}
