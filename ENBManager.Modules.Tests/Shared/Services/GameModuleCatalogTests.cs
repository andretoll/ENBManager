using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Modules.Shared.Services;
using NUnit.Framework;
using Prism.Ioc;
using System.Windows.Media.Imaging;

namespace ENBManager.Modules.Tests.Shared.Services
{
    [TestFixture]
    public class GameModuleCatalogTests
    {
        [Test]
        public void ShouldRegisterGameModule()
        {
            // Arrange
            var catalog = new GameModuleCatalog();

            // Act
            catalog.Register<Game>(null);

            // Assert
            Assert.That(catalog.GameModules, Has.Count.EqualTo(1));
        }
    }

    internal class Game : GameModule
    {
        public override string Title => throw new System.NotImplementedException();

        public override BitmapImage Icon => throw new System.NotImplementedException();

        public override string Executable => throw new System.NotImplementedException();

        public override string Module => throw new System.NotImplementedException();

        public override string[] Binaries => throw new System.NotImplementedException();

        public override string Url => throw new System.NotImplementedException();

        public override void Activate()
        {
            throw new System.NotImplementedException();
        }

        public Game(IContainerProvider containerProvider)
            : base(containerProvider)
        {
        }
    }
}
