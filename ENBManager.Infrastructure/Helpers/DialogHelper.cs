using ENBManager.Infrastructure.BusinessEntities.Dialogs.Base;
using MaterialDesignThemes.Wpf;
using System.Threading.Tasks;

namespace ENBManager.Infrastructure.Helpers
{
    public static class DialogHelper
    {
        /// <summary>
        /// Shows the current dialog and returns the result.
        /// </summary>
        /// <param name="dialog"></param>
        /// <returns></returns>
        public static bool ShowDialog(BaseDialog dialog)
        {
            CloseDialog();
            return (bool)DialogHost.Show(dialog).Result;
        }

        /// <summary>
        /// Shows the current dialog and returns the result asynchronously.
        /// </summary>
        /// <param name="dialog"></param>
        /// <returns></returns>
        public async static Task<bool> ShowDialogAsync(BaseDialog dialog)
        {
            CloseDialog();
            var result = await DialogHost.Show(dialog);

            if (result == null)
                return false;

            return (bool)result;
        }

        /// <summary>
        /// Closes the current dialog.
        /// </summary>
        public static void CloseDialog()
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }
}
