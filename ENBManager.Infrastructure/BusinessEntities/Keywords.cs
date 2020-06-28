using System.Collections.Generic;

namespace ENBManager.Infrastructure.BusinessEntities
{
    public class Keywords
    {
        #region Public Properties

        public ICollection<string> Directories { get; set; }
        public ICollection<string> Files { get; set; }

        #endregion
    }
}
