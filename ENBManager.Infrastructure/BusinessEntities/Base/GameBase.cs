using System;
using System.Windows.Media.Imaging;

namespace ENBManager.Infrastructure.BusinessEntities.Base
{
    public abstract class GameBase
    {
        public abstract string Title { get; }
        public abstract BitmapImage Icon { get; }
    }
}
