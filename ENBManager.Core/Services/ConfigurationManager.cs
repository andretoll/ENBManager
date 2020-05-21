using ENBManager.Core.Interfaces;
using ENBManager.Infrastructure.BusinessEntities.Base;
using Newtonsoft.Json;
using System;
using System.IO;

namespace ENBManager.Core.Services
{
    public class ConfigurationManager<T> : IConfigurationManager<T> where T : BaseSettings
    {
        #region Constructors

        /// <summary>
        /// Constructor. Creates a new instance of the provided type. Used to read settings directly.
        /// </summary>
        public ConfigurationManager()
        {
            Settings = (T)Activator.CreateInstance(typeof(T));
            LoadSettings();
        }

        /// <summary>
        /// Constructor. Sets the provided settings as the current settings.
        /// </summary>
        /// <param name="settings"></param>
        public ConfigurationManager(BaseSettings settings)
        {
            Settings = (T)settings;
        }

        #endregion

        #region Public Static Methods

        public static T LoadSettings(string path)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
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
                SetReadOnly(false);

            File.WriteAllText(Settings.GetFilePath(), json);
            SetReadOnly(true);
        }

        public void Initialize()
        {
            if (!File.Exists(Settings.GetFilePath()))
            {
                SaveSettings();
            }
        }

        public void SetReadOnly(bool readOnly)
        {
            if (readOnly)
                File.SetAttributes(Settings.GetFilePath(), FileAttributes.ReadOnly);
            else
                File.SetAttributes(Settings.GetFilePath(), FileAttributes.Normal);
        }

        #endregion
    }
}
