﻿using ENBManager.Configuration.Models.Base;

namespace ENBManager.Configuration.Interfaces
{
    public interface IConfigurationManager<T> where T : BaseSettings
    {
        /// <summary>
        /// The current in-memory settings.
        /// </summary>
        T Settings { get; }

        /// <summary>
        /// Loads the settings stored from the JSON file.
        /// </summary>
        /// <returns></returns>
        void LoadSettings();

        /// <summary>
        /// Saves the current settings to the JSON file.
        /// </summary>
        /// <param name="settings"></param>
        void SaveSettings();
    }
}