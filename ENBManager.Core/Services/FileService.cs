using ENBManager.Core.Interfaces;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;

namespace ENBManager.Core.Services
{
    public class FileService : IFileService
    {
        public string BrowseFile(FileType fileType)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = GetFilter(fileType)
            };
            dialog.ShowDialog();

            return dialog.FileName;
        }

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

        private string GetFilter(FileType fileType)
        {
            switch(fileType)
            {
                case FileType.Executable:
                    return "Executable files (* .exe) | *.exe";
                default:
                    throw new ArgumentException();
            }
        }
    }
}
