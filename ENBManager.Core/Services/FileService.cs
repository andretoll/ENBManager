using ENBManager.Core.Interfaces;
using ENBManager.Infrastructure.Constants;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;

namespace ENBManager.Core.Services
{
    public class FileService : IFileService
    {
        #region IFileService Implementation

        public string BrowseGameExecutable(string fileName)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = $"{fileName.Split('.').First()} ({fileName}) | {fileName}",
                CheckFileExists = true
            };

            do
            {
                dialog.ShowDialog();
            }
            while (!string.IsNullOrEmpty(dialog.FileName) &&
                Path.GetFileName(dialog.FileName) != fileName);

            return dialog.FileName;
        }

        public string[] GetGameDirectories()
        {
            var baseDirectory = Path.Combine(Paths.GetBaseDirectory(), Paths.GAMES_DIRECTORY);

            var directories = Directory.GetDirectories(baseDirectory);

            return directories;
        }

        #endregion
    }
}
