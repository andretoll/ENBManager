using System;
using System.IO;

namespace ENBManager.Infrastructure.Constants
{
    public static class Paths
    {
        #region Private Members

        private const string BASE_DIRECTORY = "ENBManager";

        #endregion

        #region Public Members

        public const string GAMES_DIRECTORY = "Games"; 

        #endregion

        #region Public Methods

        public static string GetBaseDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), BASE_DIRECTORY);
        } 

        #endregion
    }
}
