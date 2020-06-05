using System.Collections.ObjectModel;

namespace ENBManager.Modules.Shared.Models
{
    public class DirectoryItem : Item
    {
        #region Public Properties

        public ObservableCollection<Item> Items { get; set; } 

        #endregion

        #region Constructor

        public DirectoryItem()
        {
            Items = new ObservableCollection<Item>();
        } 

        #endregion
    }
}
