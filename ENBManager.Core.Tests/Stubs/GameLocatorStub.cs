using ENBManager.Core.Interfaces;
using ENBManager.TestUtils.Utils;
using System.Threading.Tasks;

namespace ENBManager.Core.Tests.Stubs
{
    public class GameLocatorStub : IGameLocator
    {
        private bool shouldMarkNextAsExisting = true;

        public Task<string> Find(string title)
        {
            if (shouldMarkNextAsExisting)
            {
                shouldMarkNextAsExisting = !shouldMarkNextAsExisting;
                return Task.FromResult(TestValues.GetRandomString());
            }
            else
                return Task.FromResult("");
        }
    }
}
