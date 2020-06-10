using ENBManager.Infrastructure.BusinessEntities.Base;
using ENBManager.Infrastructure.BusinessEntities.Dialogs.Base;
using System.Collections.Generic;

namespace ENBManager.Infrastructure.BusinessEntities.Dialogs
{
    public class TreeViewDialog : BaseDialog
    {
        #region Public Properties

        public IEnumerable<Node> Nodes { get; set; }

        #endregion

        #region Constructor

        public TreeViewDialog(string message, IEnumerable<Node> nodes)
            : base(message)
        {
            Nodes = nodes;
        }

        #endregion
    }
}
