﻿using System;
using System.IO;

namespace ENBManager.Infrastructure.Constants
{
    public static class Paths
    {
        #region Private Members

        private const string BASE_DIRECTORY = "ENBManager";
        private const string GAMES_DIRECTORY = "Games"; 
        private const string PRESETS_DIRECTORY = "Presets";
        private const string BINARIES_BACKUP_DIRECTORY = "Binaries";

        #endregion

        #region Public Methods

        public static string GetBaseDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), BASE_DIRECTORY);
        } 

        public static string GetGamesDirectory()
        {
            return Path.Combine(GetBaseDirectory(), GAMES_DIRECTORY);
        }

        public static string GetPresetsDirectory(string module)
        {
            return Path.Combine(GetBaseDirectory(), GAMES_DIRECTORY, module, PRESETS_DIRECTORY);
        }

        public static string GetBinariesBackupDirectory(string module)
        {
            return Path.Combine(GetBaseDirectory(), GAMES_DIRECTORY, module, BINARIES_BACKUP_DIRECTORY);
        }

        #endregion
    }
}
