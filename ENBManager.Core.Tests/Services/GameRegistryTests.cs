using ENBManager.Core.Services;
using NUnit.Framework;

namespace ENBManager.Core.Tests.Services
{
    [TestFixture]
    public class GameRegistryTests
    {
        [Test]
        public void ShouldGetAllSupportedGames()
        {
            // Arrange
            var registry = new GameRegistry();

            // Act
            var games = registry.GetSupportedGames();

            // Assert
            Assert.That(games, Has.Count.EqualTo(1));
        }
    }
}
