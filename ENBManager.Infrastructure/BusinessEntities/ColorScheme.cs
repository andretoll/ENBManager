using System.Windows.Media;

namespace ENBManager.Infrastructure.BusinessEntities
{
    public class ColorScheme
    {
        #region Public Properties

        public string Name { get; set; }
        public Color Primary { get; set; }
        public Color Secondary { get; set; }

        #endregion

        #region Constructor

        public ColorScheme(string name, string primaryHex, string secondaryHex)
        {
            Name = name;
            Primary = (Color)ColorConverter.ConvertFromString(primaryHex);
            Secondary = (Color)ColorConverter.ConvertFromString(secondaryHex);
        } 

        #endregion
    }
}
