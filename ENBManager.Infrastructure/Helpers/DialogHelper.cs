using ENBManager.Localization.Strings;
using NLog;
using System.IO;
using System.Windows.Forms;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace ENBManager.Infrastructure.Helpers
{
    /// <summary>
    /// A static helper class that provides functions related to <see cref="OpenFileDialog"/> and <see cref="FolderBrowserDialog"/>.
    /// </summary>
    public static class DialogHelper
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        public static string OpenDirectory()
        {
            _logger.Debug("Opening FolderBrowserDialog");

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowDialog();

            return dialog.SelectedPath;
        }

        public static string OpenExecutable()
        {
            _logger.Debug("Opening OpenFileDialog without filename");

            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "Exe Files (.exe)|*.exe",
                CheckFileExists = true
            };

            bool? cancelled;
            do
            {
                cancelled = !dialog.ShowDialog();
            }
            while (!cancelled.Value
            && !string.IsNullOrEmpty(dialog.FileName)
            && Path.GetExtension(dialog.FileName) != ".exe");

            if (cancelled.Value)
                return null;

            return dialog.FileName;
        }

        public static string OpenExecutable(string fileName)
        {
            _logger.Debug("Opening OpenFileDialog with filename");

            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = $"{fileName.Split('.')[0]} ({fileName}) | {fileName}",
                CheckFileExists = true
            };

            bool? cancelled;
            do
            {
                cancelled = !dialog.ShowDialog();
            }
            while (!cancelled.Value
            && !string.IsNullOrEmpty(dialog.FileName)
            && Path.GetFileName(dialog.FileName.ToLower()) != fileName.ToLower());

            if (cancelled.Value)
                return null;

            return dialog.FileName;
        }

        public static void SaveFile(string path, string fileName, string filter)
        {
            _logger.Debug("Opening SaveFileDialog");

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = Strings.EXPORT,
                InitialDirectory = @"C:\",
                Filter = filter,
                FileName = fileName
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = saveFileDialog.FileName;
                File.Copy(path, fileName, true);
            }
        }
    }
}
