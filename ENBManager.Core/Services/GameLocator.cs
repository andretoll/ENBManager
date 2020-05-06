using ENBManager.Core.Interfaces;
using Microsoft.Win32;
using System.Threading.Tasks;

namespace ENBManager.Core.Services
{
    public class GameLocator : IGameLocator
    {
        public async Task<string> Find(string title)
        {
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
