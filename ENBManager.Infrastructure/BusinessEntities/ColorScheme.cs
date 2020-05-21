using System.Windows.Media;

namespace ENBManager.Infrastructure.BusinessEntities
{
    public class ColorScheme
    {
        public string Name { get; set; }
        public Color Primary { get; set; }
        public Color Secondary { get; set; }

        public ColorScheme(string name, string primaryHex, string secondaryHex)
        {
            Name = name;
            Primary = (Color)ColorConverter.ConvertFromString(primaryHex);
            Secondary = (Color)ColorConverter.ConvertFromString(secondaryHex);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
