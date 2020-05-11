using ENBManager.Core.Interfaces;
using ENBManager.Core.Services;
using NUnit.Framework;
using System.Linq;

namespace ENBManager.Core.Tests.Services
{
    [TestFixture]
    public class GameRegistryTests
    {
        private IGameRegistry _gameRegistry;

        [SetUp]
        public void Setup()
        {
            _gameRegistry = new GameRegistry();
        }

        [Test]
        public void ShouldGetAllSupportedGames()
        {
            // Act
            var games = _gameRegistry.GetSupportedGames();

            // Assert
            Assert.That(games, Has.Count.EqualTo(3));
        }

        [Test]
        public void ShouldAllGamesHaveMetadata()
        {
            // Act
            var games = _gameRegistry.GetSupportedGames();

            // Assert
            Assert.That(games.All(x => !string.IsNullOrEmpty(x.Title)), Is.True);
            Assert.That(games.All(x => !string.IsNullOrEmpty(x.Executable)), Is.True);
        }
    }
}
