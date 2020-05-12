using ENBManager.Core.Interfaces;
using ENBManager.TestUtils.Utils;

namespace ENBManager.Core.Tests.Stubs
{
    public class FileServiceStub : IFileService
    {
        public string BrowseFile(FileType fileType)
        {
            return TestValues.GetRandomString();
        }

        public string BrowseGameExecutable(string fileName)
        {
            return TestValues.GetRandomString();
        }
    }
}
