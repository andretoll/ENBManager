using System.Threading.Tasks;

namespace ENBManager.Core.Interfaces
{
    public interface IGameLocator
    {
        /// <summary>
        /// Returns the install location of the specified title.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        Task<string> Find(string title);
    }
}
