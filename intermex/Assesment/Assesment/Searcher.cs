using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assesment
{
    internal class Searcher
    {
        private ConcurrentBag<string> touchedDirs = new ConcurrentBag<string>();
        private ConcurrentDictionary<string, Bitmap> iconsCache = new ConcurrentDictionary<string, Bitmap>();
        private bool cancel = true;
        private string folderIconKey = "folder";
        private Bitmap folderIcon = (Bitmap)Image.FromFile((string)Properties.Resources.ResourceManager.GetObject("FolderIcon"));
        private NodeModel rootOfTree;
        private object syncRootOfTree = new object();
        private ConcurrentBag<string> errors = new ConcurrentBag<string>();

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

            foreach (var invalidChar in Path.GetInvalidFileNameChars().Except(new [] {'*'}))
            {
                if (searchFor.Contains(invalidChar))
                {
                    var message = $"Search pattern contains invalid symbol '{invalidChar}'!";
                    Message?.Invoke(message);
                    cancel = true;
                    Finished?.Invoke(message);
                    return;
                }
            }

            if (string.IsNullOrEmpty(searchIn))
            {
                var message = "Search path can`t be empty!";
                Message?.Invoke(message);
                cancel = true;
                Finished?.Invoke(message);
                return;
            }
            if (Directory.Exists(searchIn) == false)
            {
                var message = $"Search path not found! {Environment.NewLine}{Environment.NewLine} {searchIn}";
                Message?.Invoke(message);
                cancel = true;
                Finished?.Invoke(message);
                return;
            }

            searchFor = string.IsNullOrEmpty(searchFor) ? "*.*" : searchFor;
            if (searchFor.Contains("*") == false)
            {
                searchFor = $"*{searchFor}*";
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
                void recursion(NodeModel node)
                {
                    foreach (var subNode in node.Nodes)
                    {
                        if (subNode.IsFile)
                        {
                            countOfFiles++;
                            if (directories.ContainsKey(subNode.Parent.Name) == false)
                            {
                                directories.Add(subNode.Parent.Name, subNode);
                            }
                        }

                        recursion(subNode);
                    }
                }

                if (rootOfTree != null)
                {
                    recursion(rootOfTree);
                }

                Finished?.Invoke($"[{countOfFiles} files and {directories.Count} directories found] - {searchStatus}");
                if (errors.Any())
                {
                    Message?.Invoke(string.Join(Environment.NewLine, errors));
                }
                cancel = true;
            });
        }

        private void Flush()
        {
            TryRegisterIcon(folderIconKey, folderIcon);
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
                        var cacheOfParents = new ConcurrentDictionary<string, List<DirectoryInfo>>();

                        Parallel.ForEach(foundFilesByPattern, new ParallelOptions() { MaxDegreeOfParallelism = threads },
                        foundFileItem =>
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
                                    TreeCreated?.Invoke(rootOfTree);
                                }
                            }

                            var targetModel = FindNodeInTreeByKey(dirOfFoundFileItem.FullName);

                            if (targetModel == null)
                            {
                                int indexOfFoundParent = 0;

                                while (targetModel == null && cancel == false)
                                {
                                    targetModel = FindNodeInTreeByKey(parentsUntilRoot[indexOfFoundParent].FullName);
                                    indexOfFoundParent++;
                                }
                                indexOfFoundParent--;

                                for (var i = indexOfFoundParent - 1; i >= 0; i--)
                                {
                                    targetModel = AddNodeToTree(targetModel, parentsUntilRoot[i].FullName, parentsUntilRoot[i].Name, folderIconKey);
                                }

                                targetModel = AddNodeToTree(targetModel, dirOfFoundFileItem.FullName, dirOfFoundFileItem.Name, folderIconKey);
                            }

                            // different exe files may have different icons
                            var fileIconKey = foundFileItem.EndsWith(".exe") ? foundFileItem : Path.GetExtension(foundFileItem);
                            var fileIcon = Icon.ExtractAssociatedIcon(foundFileItem).ToBitmap();
                            TryRegisterIcon(fileIconKey, fileIcon);

                            var newModel = AddNodeToTree(targetModel, foundFileItem, Path.GetFileName(foundFileItem), fileIconKey, isFile: true);
                            AddedNodeToTree?.Invoke(newModel);
                        });
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
                errors.Add($"{ex.GetType()} for {searchIn}.");
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
                ImageKey = folderIconKey, 
                SelectedImageKey = folderIconKey 
            };

            NodeModel currentNode = rootOfTree;
            for (int i = dirsUntilRoot.Count - 2; i > 0; i--)
            {
                if (cancel == true)
                {
                    return;
                }

                currentNode = AddNodeToTree(currentNode, dirsUntilRoot[i].FullName, dirsUntilRoot[i].Name, folderIconKey);
            }
        }

        private NodeModel AddNodeToTree(NodeModel target, string name, string text, string iconKey, bool isFile = false)
        {
            var newModel = new NodeModel() 
            {
                Text = text, 
                Name = name, 
                ImageKey = iconKey, 
                SelectedImageKey = iconKey,
                IsFile = isFile
            };

            target.Nodes.Add(newModel);
            newModel.Parent = target;
            return newModel;
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
            NodeModel recursion(NodeModel node)
            {
                if (node.Name == key)
                {
                    return node;
                }
                else
                {
                    foreach (var nodeItem in node.Nodes)
                    {
                        NodeModel foundNode = recursion(nodeItem);
                        if (foundNode != null)
                        {
                            return foundNode;
                        }
                    }
                }
                return null;
            }

            return recursion(rootOfTree);
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
