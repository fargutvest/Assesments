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
    public partial class Form1 : Form
    {
        private ConcurrentBag<string> touchedDirs = new ConcurrentBag<string>();
        private ConcurrentDictionary<string, Bitmap> iconsMap = new ConcurrentDictionary<string, Bitmap>();
        private TreeNode treeRoot;
        private object syncTreeRoot = new object();
        private bool cancelSearch = true;
        private string fodlerIconKey = "folder";
        private Bitmap folderIcon = (Bitmap)Bitmap.FromFile((string)Properties.Resources.ResourceManager.GetObject("FolderIcon"));
      

        public Form1()
        {
            InitializeComponent();
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            if (cancelSearch == false)
            {
                return;
            }

            cancelSearch = false;
            var searchFor = this.searchFor.Text;
            searchFor = string.IsNullOrEmpty(searchFor) ? "*.*" : searchFor;
            var searchIn = this.searchIn.Text;
            var threads = (int)countOfThreads.Value;


            if (Directory.Exists(searchIn) == false)
            {
                MessageBox.Show($"Search path not found! {Environment.NewLine}{Environment.NewLine} {searchIn}", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cancelSearch = true;
                return;
            }
            UpdateIcons(fodlerIconKey, folderIcon);

            treeView1.Nodes.Clear();
            Task.Run(() =>
            {
                touchedDirs = new ConcurrentBag<string>();
                iconsMap = new ConcurrentDictionary<string, Bitmap>();
                treeRoot = null;
                SearchRecursively(searchIn, searchFor, threads);
            }).ContinueWith(t =>
            {
                Invoke((Action)(() =>
                {
                    status.Text = cancelSearch ? "canceled" : "Complete!";
                    cancelSearch = true;
                }));
            });
        }

        private void cancelBtn_Click(object sender, EventArgs e)
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
                        UpdateStatus(dirToTouch);
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
                                    currentNode = AddNodeToTree(currentNode, parentsUntilRoot[i].FullName, parentsUntilRoot[i].Name, fodlerIconKey);
                                }

                                targetNode = AddNodeToTree(currentNode, dirOfFoundFileItem.FullName, dirOfFoundFileItem.Name, fodlerIconKey);
                            }

                            AddNodeToTree(targetNode, foundFileItem, Path.GetFileName(foundFileItem), fileIconKey);
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
                currentNode = AddNodeToTree(currentNode, parentsUntilRoot[i].FullName, parentsUntilRoot[i].Name, fodlerIconKey);
            }

            Invoke((Action)(() =>
            {
                treeView1.Nodes.Clear();
                treeRoot.ExpandAll();
                treeView1.Nodes.Add(treeRoot);
            }));
        }


        private void UpdateStatus(string message)
        {
            if (InvokeRequired)
            {
                Invoke((Action)(() =>
                {
                    status.Text = message;
                }));
            }
            else
            {
                status.Text = message;
            }
        }

        private void UpdateIcons(string key, Bitmap icon)
        {
            if (iconsMap.ContainsKey(key) == false)
            {
                iconsMap[key] = icon;

                Invoke((Action)(() =>
                {
                    if (treeView1.ImageList == null)
                    {
                        treeView1.ImageList = new ImageList();
                        treeView1.ImageList.ColorDepth = ColorDepth.Depth32Bit;
                    }
                    foreach (var item in iconsMap)
                    {
                        using (var ms = new MemoryStream())
                        {
                            treeView1.ImageList.Images.Add(key, item.Value);
                        }
                    }
                }));

            }
        }

        private TreeNode AddNodeToTree(TreeNode targetNode, string key, string value, string imageKey)
        {
            TreeNode addedNode = null;
            void add()
            {
                targetNode.ExpandAll();
                addedNode = targetNode.Nodes.Add(key, value, imageKey, imageKey);
                targetNode.ExpandAll();
                addedNode.ExpandAll();
            }

            if (InvokeRequired)
            {
                Invoke((Action)(() =>
                {
                    add();
                }));
            }
            else
            {
                add();
            }

            return addedNode;
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
