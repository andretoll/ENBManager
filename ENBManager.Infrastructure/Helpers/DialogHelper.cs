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

        public static string OpenExecutable(string fileName)
        {
            _logger.Debug("Opening OpenFileDialog");

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
            && Path.GetFileName(dialog.FileName) != fileName);

            if (cancelled.Value)
                return null;

            return dialog.FileName;
        }
    }
}
