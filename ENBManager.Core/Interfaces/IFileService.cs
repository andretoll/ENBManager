namespace ENBManager.Core.Interfaces
{
    public interface IFileService
    {
        /// <summary>
        /// Prompts user to browse the computer for a file.
        /// </summary>
        /// <param name="fileType">The type of file to browse.</param>
        /// <returns></returns>
        string BrowseFile(FileType fileType);

        /// <summary>
        /// Prompts user to browse the computer for a specific file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns></returns>
        string BrowseGameExecutable(string fileName);
    }

    public enum FileType
    {
        Executable
    }
}
