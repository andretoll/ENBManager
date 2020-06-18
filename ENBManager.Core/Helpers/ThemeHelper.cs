using ENBManager.Infrastructure.BusinessEntities;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using NLog;
using System.Collections.Generic;
using System.Linq;

namespace ENBManager.Core.Helpers
{
    /// <summary>
    /// A static helper class that provides functions related to themes and colors.
    /// </summary>
    public static class ThemeHelper
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the available color schemes.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ColorScheme> GetColorSchemes()
        {
            _logger.Debug("Getting color schemes");

            List<ColorScheme> colorSchemes = new List<ColorScheme>();

            var swatches = new SwatchesProvider().Swatches;
            colorSchemes.Add(new ColorScheme("Fallout", "#0079c9", "#e0dd02"));
            colorSchemes.Add(new ColorScheme("Flamingo", "#04ae9d", "#fe4a70"));
            colorSchemes.Add(new ColorScheme("Volcano", "#852222", "#ffb300"));
            colorSchemes.Add(new ColorScheme("Joker", "#7554A3", "#96C93C"));
            colorSchemes.Add(new ColorScheme("Nature", "#7a411d", "#7db043"));

            return colorSchemes;
        }

        /// <summary>
        /// Updates the current theme (light/dark).
        /// </summary>
        /// <param name="darkMode"></param>
        public static void UpdateTheme(bool darkMode)
        {
            _logger.Debug("Updating theme");

            var paletteHelper = new PaletteHelper();

            IBaseTheme baseTheme;
            if (darkMode)
                baseTheme = new MaterialDesignDarkTheme();
            else
                baseTheme = new MaterialDesignLightTheme();

            ITheme theme = paletteHelper.GetTheme();
            theme.SetBaseTheme(baseTheme);

            paletteHelper.SetTheme(theme);
        }

        /// <summary>
        /// Updates the current color scheme (primary/secondary).
        /// </summary>
        /// <param name="accent"></param>
        /// <param name="secondary"></param>
        public static void UpdateColorScheme(string colorSchemeName)
        {
            _logger.Debug("Updating color scheme");

            var colorScheme = GetColorScheme(colorSchemeName);
            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();
            theme.SetPrimaryColor(colorScheme.Primary);
            theme.SetSecondaryColor(colorScheme.Secondary);

            paletteHelper.SetTheme(theme);
        }

        #endregion

        #region Private Methods

        private static ColorScheme GetColorScheme(string colorSchemeName)
        {
            _logger.Debug("Getting color scheme");

            var colorSchemes = GetColorSchemes();

            var selectedColorScheme = colorSchemes.FirstOrDefault(x => x.Name == colorSchemeName);

            return selectedColorScheme ?? colorSchemes.First();
        } 

        #endregion
    }
}
