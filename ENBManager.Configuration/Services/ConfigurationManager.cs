using ENBManager.Configuration.Interfaces;
using ENBManager.Configuration.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.IO;

namespace ENBManager.Configuration.Services
{
    public class ConfigurationManager<T> : IConfigurationManager<T> where T : BaseSettings
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

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

        public static T LoadSettings(string fullPath)
        {
            _logger.Debug($"Loading settings from {fullPath}");

            var settings = JsonConvert.DeserializeObject<T>(File.ReadAllText(fullPath));

            settings.SetFullPath(fullPath);

            return settings;
        }

        #endregion

        #region IConfigurationManager Implementation

        public T Settings { get; private set; }

        public void LoadSettings()
        {
            _logger.Debug($"Loading {Settings.GetType().Name}");

            // If directory or file does not exist, create it
            InitializeSettings();

            Settings = JsonConvert.DeserializeObject<T>(File.ReadAllText(Settings.GetFullPath()));
        }

        public void SaveSettings()
        {
            _logger.Debug($"Saving {Settings.GetType().Name}");

            // If directory does not exist, create it
            Directory.CreateDirectory(Path.GetDirectoryName(Settings.GetFullPath()));

            // If file exists, unlock it
            if (File.Exists(Settings.GetFullPath()))
                SetReadOnly(false);

            string json = JsonConvert.SerializeObject(Settings);
            File.WriteAllText(Settings.GetFullPath(), json);

            SetReadOnly(true);
        }

        public void InitializeSettings()
        {
            if (!File.Exists(Settings.GetFullPath()))
            {
                _logger.Debug($"Initializing {Settings.GetType().Name}");

                SaveSettings();
            }
        }

        public void SetReadOnly(bool readOnly)
        {
            _logger.Debug(nameof(SetReadOnly) + " = " + readOnly);

            File.SetAttributes(Settings.GetFullPath(), readOnly ? FileAttributes.ReadOnly : FileAttributes.Normal);
        }

        #endregion
    }
}
