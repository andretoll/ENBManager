using Microsoft.Win32;
using NLog;
using System.Threading.Tasks;

namespace ENBManager.Core.Helpers
{
    /// <summary>
    /// A static helper class that provides functions related to locate installed games.
    /// </summary>
    public static class GameLocator
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static async Task<string> Find(string title)
        {
            _logger.Debug($"Attempts to find game '{title}'");

            string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
            {
                foreach (var subkeyName in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkeyName))
                    {
                        if ((subkey.GetValue("DisplayName") as string) == title)
                            return await Task.FromResult(subkey.GetValue("InstallLocation") as string);
                    }
                }
            }

            return await Task.FromResult(string.Empty);
        }
    }
}
