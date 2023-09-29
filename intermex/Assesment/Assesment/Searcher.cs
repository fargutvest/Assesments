using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assesment
{
    internal class Searcher
    {
        private ConcurrentBag<string> touchedDirs = new ConcurrentBag<string>();
        private object syncTreeRoot = new object();
        private bool cancelSearch = true;
        private string fodlerIconKey = "folder";
        private Bitmap folderIcon = (Bitmap)Bitmap.FromFile((string)Properties.Resources.ResourceManager.GetObject("FolderIcon"));
        private ConcurrentDictionary<string, Bitmap> iconsMap;
        private TreeNode treeRoot;
       
        public event Action<string> SearchCompleted;
        public event Action<TreeNode> TreeRootInited;
        public event Action<string> Progress;
        public event Action<string, Bitmap> IconAdded;
        public event Action<string> Message;
        public event Func<TreeNode, string, string, string, TreeNode> AddNodeToTree;


        public void Start(string searchFor, string searchIn, int countOfThreads)
        {
            if (cancelSearch == false)
            {
                return;
            }

            cancelSearch = false;

            if (Directory.Exists(searchIn) == false)
            {
                Message?.Invoke($"Search path not found! {Environment.NewLine}{Environment.NewLine} {searchIn}");
                cancelSearch = true;
                return;
            }

            touchedDirs = new ConcurrentBag<string>();
            iconsMap = new ConcurrentDictionary<string, Bitmap>();
            treeRoot = null;
            UpdateIcons(fodlerIconKey, folderIcon);

            Task.Run(() =>
            {
                SearchRecursively(searchIn, searchFor, countOfThreads);
            }).ContinueWith(t =>
            {
                SearchCompleted?.Invoke(cancelSearch ? "canceled" : "Complete!");
                cancelSearch = true;
            });
        }

        public void Cancel()
        {
            cancelSearch = true;
        }


        private void SearchRecursively(string searchIn, string searchFor, int threads)
        {
            try
            {
                var allExistedFiles = Directory.EnumerateFiles(searchIn);
                Parallel.ForEach(allExistedFiles, new ParallelOptions() { MaxDegreeOfParallelism = threads },
                file =>
                {
                    if (cancelSearch == true)
                    {
                        return;
                    }

                    var dirToTouch = Path.GetDirectoryName(file);
                    if (touchedDirs.Contains(dirToTouch) == false)
                    {
                        Progress?.Invoke(dirToTouch);
                        touchedDirs.Add(dirToTouch);
                        var foundFilesByPattern = Directory.EnumerateFiles(dirToTouch, searchFor).ToList();

                        Dictionary<string, List<DirectoryInfo>> cacheOfParents = new Dictionary<string, List<DirectoryInfo>>();
                        foreach (var foundFileItem in foundFilesByPattern)
                        {
                            if (cancelSearch == true)
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

                            // different exe files may have different icons
                            var fileIconKey = foundFileItem.EndsWith(".exe") ? foundFileItem : Path.GetExtension(foundFileItem);
                            var fileIcon = Icon.ExtractAssociatedIcon(foundFileItem).ToBitmap();
                            UpdateIcons(fileIconKey, fileIcon);

                            lock (syncTreeRoot)
                            {
                                if (treeRoot == null)
                                {
                                    InitTreeRootByFoundFile(parentsUntilRoot, dirOfFoundFileItem);
                                }
                            }

                            var targetNode = FindNodeInTreeByKey(dirOfFoundFileItem.FullName);

                            if (targetNode == null)
                            {
                                int i = 0;
                                while (targetNode == null && cancelSearch == false)
                                {
                                    targetNode = FindNodeInTreeByKey(parentsUntilRoot[i].FullName);
                                    i++;
                                }
                                int indexOfFoundParent = i - 1;

                                TreeNode currentNode = targetNode;

                                for (i = indexOfFoundParent - 1; i >= 0; i--)
                                {
                                    currentNode = AddNodeToTree?.Invoke(currentNode, parentsUntilRoot[i].FullName, parentsUntilRoot[i].Name, fodlerIconKey);
                                }

                                targetNode = AddNodeToTree?.Invoke(currentNode, dirOfFoundFileItem.FullName, dirOfFoundFileItem.Name, fodlerIconKey);
                            }

                            AddNodeToTree?.Invoke(targetNode, foundFileItem, Path.GetFileName(foundFileItem), fileIconKey);
                        }
                    }
                });

                foreach (var item in Directory.GetDirectories(searchIn))
                {
                    if (cancelSearch == true)
                    {
                        return;
                    }
                    SearchRecursively(item, searchFor, threads);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void InitTreeRootByFoundFile(List<DirectoryInfo> parentsUntilRoot, DirectoryInfo dirOfFoundFileItem)
        {
            var rootSearchIn = parentsUntilRoot.Any() ? parentsUntilRoot.Last() : dirOfFoundFileItem;

            treeRoot = new TreeNode(rootSearchIn.FullName) { Name = rootSearchIn.FullName, ImageKey = fodlerIconKey, SelectedImageKey = fodlerIconKey };
            TreeNode currentNode = treeRoot;
            for (int i = parentsUntilRoot.Count - 2; i >= 0; i--)
            {
                if (cancelSearch == true)
                {
                    return;
                }
                currentNode = AddNodeToTree?.Invoke(currentNode, parentsUntilRoot[i].FullName, parentsUntilRoot[i].Name, fodlerIconKey);
            }

            TreeRootInited?.Invoke(treeRoot);
        }

        private void UpdateIcons(string key, Bitmap icon)
        {
            if (iconsMap.ContainsKey(key) == false)
            {
                iconsMap[key] = icon;
                IconAdded?.Invoke(key, icon);
            }
        }

        private TreeNode FindNodeInTreeByKey(string key)
        {
            if (treeRoot.Name == key)
            {
                return treeRoot;
            }

            var foundTreeNodes = treeRoot.Nodes.Find(key, searchAllChildren: true);

            return foundTreeNodes.FirstOrDefault();
        }

        private List<DirectoryInfo> GetParentsUntilRoot(DirectoryInfo directoryInfo)
        {
            DirectoryInfo root = directoryInfo.Root;
            DirectoryInfo parent = directoryInfo;
            var parents = new List<DirectoryInfo>();
            while (parent.FullName != root.FullName && cancelSearch == false)
            {
                parent = parent.Parent;
                parents.Add(parent);
            }
            return parents;
        }

    }
}
