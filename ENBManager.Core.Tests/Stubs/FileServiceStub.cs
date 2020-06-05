using ENBManager.Core.Interfaces;
using ENBManager.TestUtils.Utils;

namespace ENBManager.Core.Tests.Stubs
{
    public class FileServiceStub : IGameService
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
    }
}
