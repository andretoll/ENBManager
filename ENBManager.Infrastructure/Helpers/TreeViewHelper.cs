using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.BusinessEntities.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace ENBManager.Infrastructure.Helpers
{
    public static class TreeViewHelper
    {
        public static List<Node> GetItems(string root, PropertyChangedEventHandler eventHandler)
        {
            var items = new List<Node>();

            var dirInfo = new DirectoryInfo(root);

            foreach (var directory in dirInfo.GetDirectories())
            {
                var item = new DirectoryNode
                {
                    Name = directory.Name,
                    Path = directory.FullName,
                    Items = new ObservableCollection<Node>(GetItems(directory.FullName, eventHandler))
                };

                item.PropertyChanged += eventHandler;

                items.Add(item);
            }

            foreach (var file in dirInfo.GetFiles())
            {
                var item = new FileNode
                {
                    Name = file.Name,
                    Path = file.FullName
                };

                item.PropertyChanged += eventHandler;

                items.Add(item);
            }

            return items;
        }

        public static List<Node> GetItems(string root)
        {
            var items = new List<Node>();

            var dirInfo = new DirectoryInfo(root);

            foreach (var directory in dirInfo.GetDirectories())
            {
                var item = new DirectoryNode
                {
                    Name = directory.Name,
                    Path = directory.FullName,
                    Items = new ObservableCollection<Node>(GetItems(directory.FullName))
                };

                items.Add(item);
            }

            foreach (var file in dirInfo.GetFiles())
            {
                var item = new FileNode
                {
                    Name = file.Name,
                    Path = file.FullName
                };

                items.Add(item);
            }

            return items;
        }

        public static ICollection<string> GetPaths(DirectoryNode directory)
        {
            List<string> paths = new List<string>();

            // Add empty directory
            if (directory.Items.Count == 0)
                paths.Add(directory.Path);

            foreach (var item in directory.Items.Where(x => x.GetType() == typeof(DirectoryNode)))
            {
                paths.AddRange(GetPaths(item as DirectoryNode));
            }

            foreach (var file in directory.Items.Where(x => x.GetType() == typeof(FileNode)))
            {
                paths.Add(file.Path);
            }

            return paths;
        }

        public static void DeleteItem(ICollection<Node> nodes, Node nodeToDelete)
        {
            if (!nodes.Remove(nodeToDelete))
            {
                foreach (var node in nodes.Where(x => x.GetType() == typeof(DirectoryNode)))
                {
                    DeleteItem((node as DirectoryNode).Items, nodeToDelete);
                }
            }
        }
    }
}
