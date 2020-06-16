using ENBManager.Infrastructure.Enums;

namespace ENBManager.Infrastructure.Interfaces
{
    public interface IGameService
    {
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
        bool VerifyBinaries(string directoryPath, string[] files, out string[] missingBinaries);

        /// <summary>
        /// Verifies the existance of the files provided.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        bool VerifyBinaries(string directoryPath, string[] files);

        /// <summary>
        /// Verifies the version of the provided binaries compared to the binaries in the game directory.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="binaries"></param>
        /// <returns></returns>
        VersionMismatch VerifyBinariesVersion(string source, string target, string[] binaries);

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
