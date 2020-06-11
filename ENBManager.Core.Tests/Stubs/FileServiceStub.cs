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

        public void CopyBinaries(string source, string target, string[] binaries)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteBinaries(string target, string[] binaries)
        {
            throw new System.NotImplementedException();
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

        public bool VerifyBinariesBackup(string directoryPath, params string[] binaries)
        {
            throw new System.NotImplementedException();
        }
    }
}
