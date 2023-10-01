using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assesment
{
    internal class Searcher
    {
        private ConcurrentBag<string> touchedDirs = new ConcurrentBag<string>();
        private ConcurrentDictionary<string, Bitmap> iconsCache = new ConcurrentDictionary<string, Bitmap>();
        private bool cancel = true;
        private string fodlerIconKey = "folder";
        private Bitmap folderIcon = (Bitmap)Image.FromFile((string)Properties.Resources.ResourceManager.GetObject("FolderIcon"));
        private NodeModel rootOfTree;
        private object syncRootOfTree = new object();

        public event Action<NodeModel> TreeCreated;
        public event Action<NodeModel> AddedNodeToTree;
        public event Action<string, Bitmap> IconAdded;
        public event Action<string> Progress;
        public event Action<string> Finished;
        public event Action<string> Message;


        public void Start(string searchFor, string searchIn, int countOfThreads)
        {
            if (cancel == false) // if already started then do nothing
            {
                return;
            }

            cancel = false;

            if (Directory.Exists(searchIn) == false)
            {
                Message?.Invoke($"Search path not found! {Environment.NewLine}{Environment.NewLine} {searchIn}");
                cancel = true;
                return;
            }

            Flush();

            Task.Run(() =>
            {
                SearchRecursively(searchIn, searchFor, countOfThreads);
            }).ContinueWith(t =>
            {
                int countOfFiles = 0;
                string searchStatus = cancel ? "search canceled" : "search completed";

                Dictionary<string, NodeModel> directories = new Dictionary<string, NodeModel>();
                void recurse(NodeModel node)
                {
                    foreach (var subNode in node.Nodes)
                    {
                        if (subNode.IsFile)
                        {
                            countOfFiles ++;
                            if (directories.ContainsKey(subNode.Parent.Name) == false)
                            {
                                directories.Add(subNode.Parent.Name, subNode);
                            }
                        }

                        recurse(subNode);
                    }
                }

                recurse(rootOfTree);

                Finished?.Invoke($"[{countOfFiles} files and {directories.Count} directories found] - {searchStatus}");
                cancel = true;
            });
        }

        private void Flush()
        {
            TryRegisterIcon(fodlerIconKey, folderIcon);
            touchedDirs = new ConcurrentBag<string>();
            rootOfTree = null;
        }

        public void Cancel()
        {
            cancel = true;
        }


        private void SearchRecursively(string searchIn, string searchPattern, int threads)
        {
            try
            {
                var allExistedFilesQuery = Directory.EnumerateFiles(searchIn);
                Parallel.ForEach(allExistedFilesQuery, new ParallelOptions() { MaxDegreeOfParallelism = threads },
                file =>
                {
                    if (cancel == true)
                    {
                        return;
                    }

                    var dirToTouch = Path.GetDirectoryName(file);
                    if (touchedDirs.Contains(dirToTouch) == false)
                    {
                        touchedDirs.Add(dirToTouch);
                        Progress?.Invoke(dirToTouch);

                        var foundFilesByPattern = Directory.EnumerateFiles(dirToTouch, searchPattern);
                        var cacheOfParents = new Dictionary<string, List<DirectoryInfo>>();

                        foreach (var foundFileItem in foundFilesByPattern)
                        {
                            if (cancel == true)
                            {
                                return;
                            }

                            var dirOfFoundFileItem = new FileInfo(foundFileItem).Directory;

                            List<DirectoryInfo> parentsUntilRoot = null;
                            if (cacheOfParents.ContainsKey(dirOfFoundFileItem.FullName))
                            {
                                parentsUntilRoot = cacheOfParents[dirOfFoundFileItem.FullName];
                            }
                            else
                            {
                                parentsUntilRoot = GetParentsUntilRoot(dirOfFoundFileItem);
                            }

                            lock (syncRootOfTree)
                            {
                                if (rootOfTree == null)
                                {
                                    var dirsUntilRoot = new List<DirectoryInfo>
                                    {
                                        dirOfFoundFileItem
                                    };
                                    dirsUntilRoot.AddRange(parentsUntilRoot);
                                    CreateTreeOfNodes(dirsUntilRoot);
                                }
                            }

                            var targetNode = FindNodeInTreeByKey(dirOfFoundFileItem.FullName);

                            if (targetNode == null)
                            {
                                int indexOfFoundParent = 0;
                               
                                while (targetNode == null && cancel == false)
                                {
                                    targetNode = FindNodeInTreeByKey(parentsUntilRoot[indexOfFoundParent].FullName);
                                    indexOfFoundParent++;
                                }
                                indexOfFoundParent--;

                                for (var i = indexOfFoundParent - 1; i >= 0; i--)
                                {
                                    targetNode = AddNodeToTree(targetNode, parentsUntilRoot[i].FullName, parentsUntilRoot[i].Name, fodlerIconKey);
                                }

                                targetNode = AddNodeToTree(targetNode, dirOfFoundFileItem.FullName, dirOfFoundFileItem.Name, fodlerIconKey);
                            }

                            // different exe files may have different icons
                            var fileIconKey = foundFileItem.EndsWith(".exe") ? foundFileItem : Path.GetExtension(foundFileItem);
                            var fileIcon = Icon.ExtractAssociatedIcon(foundFileItem).ToBitmap();
                            TryRegisterIcon(fileIconKey, fileIcon);

                            AddNodeToTree(targetNode, foundFileItem, Path.GetFileName(foundFileItem), fileIconKey, isFile: true);
                        }
                    }
                });

                Parallel.ForEach(Directory.GetDirectories(searchIn), new ParallelOptions() { MaxDegreeOfParallelism = threads },
                item =>
                {
                    if (cancel == true)
                    {
                        return;
                    }
                    SearchRecursively(item, searchPattern, threads);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void CreateTreeOfNodes(List<DirectoryInfo> dirsUntilRoot)
        {
            var rootDir = dirsUntilRoot.Last();

            rootOfTree = new NodeModel() 
            {
                Name = rootDir.FullName, 
                Text = rootDir.Name, 
                ImageKey = fodlerIconKey, 
                SelectedImageKey = fodlerIconKey 
            };

            NodeModel currentNode = rootOfTree;
            for (int i = dirsUntilRoot.Count - 1; i > 0; i--)
            {
                if (cancel == true)
                {
                    return;
                }

                currentNode = AddNodeToTree(currentNode, dirsUntilRoot[i].FullName, dirsUntilRoot[i].Name, fodlerIconKey);
            }

            TreeCreated?.Invoke(rootOfTree);
        }

        private NodeModel AddNodeToTree(NodeModel target, string name, string text, string iconKey, bool isFile = false)
        {
            var newNode = new NodeModel() 
            {
                Text = text, 
                Name = name, 
                ImageKey = iconKey, 
                SelectedImageKey = iconKey,
                IsFile = isFile
            };

            target.Nodes.Add(newNode);
            newNode.Parent = target;
            AddedNodeToTree?.Invoke(newNode);
            return newNode;
        }

        private void TryRegisterIcon(string key, Bitmap icon)
        {
            if (iconsCache.ContainsKey(key) == false)
            {
                iconsCache[key] = icon;
                IconAdded?.Invoke(key, icon);
            }
        }

        private NodeModel FindNodeInTreeByKey(string key)
        {
            NodeModel recurse(NodeModel node)
            {
                if (node.Name == key)
                {
                    return node;
                }
                else
                {
                    foreach (var nodeItem in node.Nodes)
                    {
                        NodeModel foundNode = recurse(nodeItem);
                        if (foundNode != null)
                        {
                            return foundNode;
                        }
                    }
                }
                return null;
            }

            return recurse(rootOfTree);
        }

        private List<DirectoryInfo> GetParentsUntilRoot(DirectoryInfo directoryInfo)
        {
            DirectoryInfo root = directoryInfo.Root;
            DirectoryInfo parent = directoryInfo;
            var parents = new List<DirectoryInfo>();
            while (parent.FullName != root.FullName && cancel == false)
            {
                parent = parent.Parent;
                parents.Add(parent);
            }
            return parents;
        }

    }
}
