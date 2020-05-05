using ENBManager.Configuration.Models.Base;

namespace ENBManager.Configuration.Interfaces
{
    public interface IConfigurationManager<T> where T : BaseSettings
    {
        /// <summary>
        /// Loads the settings stored from the JSON file.
        /// </summary>
        /// <returns></returns>
        T LoadSettings();

        /// <summary>
        /// Applies the provided settings.
        /// </summary>
        /// <param name="settings"></param>
        void ApplySettings(T settings);

        /// <summary>
        /// Saves the current settings to the JSON file.
        /// </summary>
        /// <param name="settings"></param>
        void SaveSettings();
    }
}
