using ENBManager.Core.BusinessEntities.Base;
using ENBManager.Core.Interfaces;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace ENBManager.Core.Services
{
    public class GameRegistry : IGameRegistry
    {
        private readonly List<GameBase> _supportedGames;

        public GameRegistry()
        {
            _supportedGames = new List<GameBase>();

            _supportedGames.Add(new SkyrimSE());
        }

        public IEnumerable<GameBase> GetSupportedGames()
        {
            return _supportedGames;
        }
    }

    internal class SkyrimSE : GameBase
    {
        public override string Title => "The Elder Scrolls V: Skyrim Special Edition";
        public override string Executable => "SkyrimSE.exe";
        public override string InstalledLocation { get; set; }
        public override BitmapImage Icon { get; set; }
    }
}
