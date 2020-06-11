using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.BusinessEntities.Base;
using NLog;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace ENBManager.Infrastructure.Helpers
{
    /// <summary>
    /// A static helper class that provides functions related to treeviews.
    /// </summary>
    public static class TreeViewHelper
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Public Methods

        public static List<Node> GetNodes(string root, PropertyChangedEventHandler eventHandler)
        {
            _logger.Debug("Getting nodes with event handler");

            var items = new List<Node>();

            var dirInfo = new DirectoryInfo(root);

            foreach (var directory in dirInfo.GetDirectories())
            {
                var item = new DirectoryNode
                {
                    Name = directory.Name,
                    Path = directory.FullName,
                    Items = new ObservableCollection<Node>(GetNodes(directory.FullName, eventHandler))
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

        public static List<Node> GetNodes(string root)
        {
            _logger.Debug("Getting nodes");

            var items = new List<Node>();

            var dirInfo = new DirectoryInfo(root);

            foreach (var directory in dirInfo.GetDirectories())
            {
                var item = new DirectoryNode
                {
                    Name = directory.Name,
                    Path = directory.FullName,
                    Items = new ObservableCollection<Node>(GetNodes(directory.FullName))
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
            _logger.Debug("Getting paths");

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

        public static void DeleteNode(ICollection<Node> nodes, Node nodeToDelete)
        {
            _logger.Debug($"Deleting node {nodeToDelete.Name}");

            if (!nodes.Remove(nodeToDelete))
            {
                foreach (var node in nodes.Where(x => x.GetType() == typeof(DirectoryNode)))
                {
                    DeleteNode((node as DirectoryNode).Items, nodeToDelete);
                }
            }
        } 

        #endregion
    }
}
