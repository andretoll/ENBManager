using ENBManager.Core.Interfaces;
using Microsoft.Win32;
using NLog;
using System.Threading.Tasks;

namespace ENBManager.Core.Services
{
    public class GameLocator : IGameLocator
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public async Task<string> Find(string title)
        {
            _logger.Debug(nameof(Find));

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
