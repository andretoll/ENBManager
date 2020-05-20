using MaterialDesignThemes.Wpf;

namespace ENBManager.Core.Helpers
{
    public static class ThemeHelper
    {
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
    }
}
