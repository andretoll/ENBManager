using ENBManager.Infrastructure.BusinessEntities;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Linq;

namespace ENBManager.Core.Helpers
{
    public static class ThemeHelper
    {
        /// <summary>
        /// Gets the available color schemes.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ColorScheme> GetColorSchemes()
        {
            List<ColorScheme> colorSchemes = new List<ColorScheme>();

            var swatches = new SwatchesProvider().Swatches;
            colorSchemes.Add(new ColorScheme("Air", "#c3d5e0", "#8fc3e3"));
            colorSchemes.Add(new ColorScheme("Earth", "#7a411d", "#7db043"));
            colorSchemes.Add(new ColorScheme("Fire", "#852222", "#ffb300"));
            colorSchemes.Add(new ColorScheme("Ice", "#6f727a", "#26c6da"));

            return colorSchemes;
        }

        /// <summary>
        /// Updates the current theme (light/dark).
        /// </summary>
        /// <param name="darkMode"></param>
        public static void UpdateTheme(bool darkMode)
        {
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
            var colorScheme = GetColorScheme(colorSchemeName);
            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();
            theme.SetPrimaryColor(colorScheme.Primary);
            theme.SetSecondaryColor(colorScheme.Secondary);

            paletteHelper.SetTheme(theme);
        }

        private static ColorScheme GetColorScheme(string colorSchemeName)
        {
            var colorSchemes = GetColorSchemes();

            var selectedColorScheme = colorSchemes.FirstOrDefault(x => x.Name == colorSchemeName);

            return selectedColorScheme != null ? selectedColorScheme : colorSchemes.First();
        }
    }
}
