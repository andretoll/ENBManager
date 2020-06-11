using ENBManager.Infrastructure.Enums;

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
        /// Retrieves the managed games defined by existing folders in ProgramData.
        /// </summary>
        /// <returns></returns>
        string[] GetGameDirectories();

        /// <summary>
        /// Deletes a managed game directory from disk.
        /// </summary>
        void DeleteGameDirectory(string directoryName);

        /// <summary>
        /// Verifies the existance of the files provided. Returns any missing files.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        string[] VerifyBinaries(string directoryPath, string[] files);

        /// <summary>
        /// Verifies the version of the provided binaries compared to the binaries in the game directory.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="binaries"></param>
        /// <returns></returns>
        VersionMismatch VerifyBinariesVersion(string source, string target, string[] binaries);

        /// <summary>
        /// Verifies the existance of binaries backup.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="binaries"></param>
        /// <returns></returns>
        bool VerifyBinariesBackup(string directoryPath, params string[] binaries);

        /// <summary>
        /// Creates backup files of the provided files.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="binaries"></param>
        void CopyBinaries(string source, string target, string[] binaries);

        /// <summary>
        /// Deletes the binaries from the target directory.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="binaries"></param>
        void DeleteBinaries(string target, string[] binaries);
    }
}
