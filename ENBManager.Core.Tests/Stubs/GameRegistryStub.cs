using ENBManager.Core.BusinessEntities;
using ENBManager.Core.Interfaces;
using ENBManager.TestUtils.Utils;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace ENBManager.Core.Tests.Stubs
{
    public class GameRegistryStub : IGameRegistry
    {
        public IEnumerable<InstalledGame> GetSupportedGames()
        {
            return new List<InstalledGame>()
            {
                new Game1(),
                new Game2()
            };
        }
    }

    internal class Game1 : InstalledGame
    {
        public override string Executable => "Game1.exe";

        public override string Title => "Game1";

        public override string InstalledLocation { get; set; }

        public override BitmapImage Icon => TestValues.GetRandomImage();
    }

    internal class Game2 : InstalledGame
    {
        public override string Executable => "Game2.exe";

        public override string Title => "Game2";

        public override string InstalledLocation { get; set; }

        public override BitmapImage Icon => TestValues.GetRandomImage();
    }
}
