namespace ENBManager.Infrastructure.Interfaces
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

        /// <summary>
        /// Verifies the existance of the files provided. Returns any missing files.
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        string[] VerifyBinaries(string directoryPath, string[] files);
    }
}
