using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.Constants;
using ENBManager.Infrastructure.Enums;
using ENBManager.Infrastructure.Exceptions;
using Newtonsoft.Json;
using NLog;
using System.Collections.Generic;
using System.IO;

namespace ENBManager.Infrastructure.Helpers
{
    public static class KeywordsHelper
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static Keywords GetKeywords()
        {
            _logger.Debug("Getting keywords");

            if (!File.Exists(Paths.GetKeywordsFilePath()))
            {
                var keywords = new Keywords();
                keywords.Directories = DefaultKeywords.DIRECTORIES;
                keywords.Files = DefaultKeywords.FILES;

                SaveToFile(keywords);
            }

            return JsonConvert.DeserializeObject<Keywords>(File.ReadAllText(Paths.GetKeywordsFilePath()));
        }

        public static void AddKeyword(KeywordType keywordType, string value)
        {
            _logger.Debug("Adding keyword");

            var keywords = GetKeywords();

            switch (keywordType)
            {
                case KeywordType.Folder:
                    if (keywords.Directories.Contains(value))
                        throw new IdenticalNameException($"Keyword with name '{value}' already exists");
                    keywords.Directories.Add(value);
                    break;
                case KeywordType.File:
                    if (keywords.Files.Contains(value))
                        throw new IdenticalNameException($"Keyword with name '{value}' already exists");
                    keywords.Files.Add(value);
                    break;
            }

            SaveToFile(keywords);
        }

        public static void RemoveKeyword(KeywordType keywordType, string value)
        {
            _logger.Debug("Removing keyword");

            var keywords = GetKeywords();

            switch (keywordType)
            {
                case KeywordType.Folder:
                    keywords.Directories.Remove(value);
                    break;
                case KeywordType.File:
                    keywords.Files.Remove(value);
                    break;
            }

            SaveToFile(keywords);
        }

        public static bool MatchesKeyword(IEnumerable<string> keywords, string name)
        {
            _logger.Debug("Is keyword matching?");

            foreach (var keyword in keywords)
            {
                if (name.ToLower().Contains(keyword.ToLower()))
                    return true;
            }

            return false;
        }

        #region Helper Methods

        private static void SaveToFile(Keywords keywords)
        {
            _logger.Debug("Saving keywords to file");

            if (!Directory.Exists(Path.GetDirectoryName(Paths.GetKeywordsFilePath())))
                Directory.CreateDirectory(Path.GetDirectoryName(Paths.GetKeywordsFilePath()));

            File.WriteAllText(Paths.GetKeywordsFilePath(), JsonConvert.SerializeObject(keywords, Formatting.Indented));
        }

        #endregion
    }
}
