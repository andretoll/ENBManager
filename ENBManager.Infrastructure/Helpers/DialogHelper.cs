using System.Windows.Forms;

namespace ENBManager.Infrastructure.Helpers
{
    public static class DialogHelper
    {
        public static bool OpenFolderDialog(out string path)
        {
            path = null;

            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() != DialogResult.OK)
                return false;

            path = dialog.SelectedPath;
            return true;
        }
    }
}
