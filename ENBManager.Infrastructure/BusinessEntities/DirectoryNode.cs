using ENBManager.Infrastructure.BusinessEntities.Base;
using System.Collections.ObjectModel;

namespace ENBManager.Infrastructure.BusinessEntities
{
    public class DirectoryNode : Node
    {
        #region Public Properties

        public ObservableCollection<Node> Items { get; set; } 

        #endregion

        #region Constructor

        public DirectoryNode()
        {
            Items = new ObservableCollection<Node>();
        } 

        #endregion
    }
}
