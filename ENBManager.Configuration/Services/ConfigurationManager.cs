using ENBManager.Configuration.Interfaces;
using ENBManager.Configuration.Models.Base;
using Newtonsoft.Json;
using System;
using System.IO;

namespace ENBManager.Configuration.Services
{
    public class ConfigurationManager<T> : IConfigurationManager<T> where T : BaseSettings
    {
        #region Constructors

        public ConfigurationManager()
        {
            Settings = (T)Activator.CreateInstance(typeof(T));
            LoadSettings();
        }

        public ConfigurationManager(BaseSettings settings)
        {
            Settings = (T)settings;
        }

        #endregion

        #region IConfigurationManager Implementation

        public T Settings { get; private set; }

        public void LoadSettings()
        {
            // If directory or file does not exist, create it
            Initialize();

            Settings = JsonConvert.DeserializeObject<T>(File.ReadAllText(Settings.GetFilePath()));
        }

        public void SaveSettings()
        {
            // If directory does not exist, create it
            if (!Directory.Exists(Path.GetDirectoryName(Settings.GetFilePath())))
                Directory.CreateDirectory(Path.GetDirectoryName(Settings.GetFilePath()));

            string json = JsonConvert.SerializeObject(Settings);

            // If file exists, unlock it
            if (File.Exists(Settings.GetFilePath()))
                File.SetAttributes(Settings.GetFilePath(), FileAttributes.Normal);

            File.WriteAllText(Settings.GetFilePath(), json);
            File.SetAttributes(Settings.GetFilePath(), FileAttributes.ReadOnly);
        }

        public void Initialize()
        {
            if (!File.Exists(Settings.GetFilePath()))
            {
                SaveSettings();
            }
        }

        #endregion
    }
}
