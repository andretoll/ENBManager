using ENBManager.Configuration.Interfaces;
using ENBManager.Configuration.Models.Base;
using Newtonsoft.Json;
using System;
using System.IO;

namespace ENBManager.Configuration.Services
{
    public class ConfigurationManager<T> : IConfigurationManager<T> where T : BaseSettings
    {
        #region Private Members

        private BaseSettings _settings;

        #endregion

        #region Constructor

        public ConfigurationManager()
        {
            _settings = (BaseSettings)Activator.CreateInstance(typeof(T));
        }

        #endregion

        #region IConfigurationManager Implementation

        public T LoadSettings()
        {
            // If directory does not exist, create it
            if (!Directory.Exists(Path.GetDirectoryName(_settings.GetFilePath())))
                SaveSettings();

            return JsonConvert.DeserializeObject<T>(File.ReadAllText(_settings.GetFilePath()));
        }

        public void ApplySettings(T settings)
        {
            _settings = settings;
        }

        public void SaveSettings()
        {
            // If directory does not exist, create it
            if (!Directory.Exists(Path.GetDirectoryName(_settings.GetFilePath())))
                Directory.CreateDirectory(Path.GetDirectoryName(_settings.GetFilePath()));

            string json = JsonConvert.SerializeObject(_settings);
            File.WriteAllText(_settings.GetFilePath(), json);
        }

        #endregion
    }
}
