using System.Windows.Media.Imaging;

namespace ENBManager.Core.BusinessEntities.Base
{
    public abstract class GameBase
    {
        public abstract string Title { get; }
        public abstract string Executable { get; }
        public abstract string InstalledLocation { get; set; }
        public abstract BitmapImage Icon { get; }

        public bool Installed => !string.IsNullOrEmpty(InstalledLocation);
    }
}
