using ENBManager.Modules.Shared.Models.Base;
using System.Collections.ObjectModel;

namespace ENBManager.Modules.Shared.Models
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
