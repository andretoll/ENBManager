using ENBManager.Infrastructure.Interfaces;
using ENBManager.TestUtils.Utils;

namespace ENBManager.Core.Tests.Stubs
{
    public class GameServiceStub : IGameService
    {
        public string BrowseGameExecutable(string fileName)
        {
            return TestValues.GetRandomString();
        }

        public void DeleteGameDirectory(string directoryName)
        {
            throw new System.NotImplementedException();
        }

        public string[] GetGameDirectories()
        {
            throw new System.NotImplementedException();
        }

        public string[] VerifyBinaries(string directoryPath, string[] files)
        {
            throw new System.NotImplementedException();
        }
    }
}
