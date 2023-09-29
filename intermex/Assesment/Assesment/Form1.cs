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

            Task.Run(() =>
            {
                var parents = GetParentDirectoriesTreeList(new DirectoryInfo(searchIn));
                var first = parents.First().FullName;
                treeRoot = new TreeNode(first) { Name = first, ImageKey = fodlerIconKey, SelectedImageKey = fodlerIconKey };
                TreeNode currentNode = treeRoot;
                foreach (var item in parents.Skip(1))
                {
                    var newNode = AddNodeToTree(currentNode, item.FullName, item.Name, fodlerIconKey);
                    currentNode = newNode;
                }

                Invoke((Action)(() =>
                {
                    treeView1.Nodes.Clear();
                    treeRoot.ExpandAll();
                    treeView1.Nodes.Add(treeRoot);
                }));

                Search(searchIn, searchFor, threads);
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


        private void Search(string searchIn, string searchFor, int threads)
        {
            try
            {
                touchedDirs = new ConcurrentBag<string>();
                iconsMap = new ConcurrentDictionary<string, Bitmap>();

                var allExistedFiles = Directory.EnumerateFiles(searchIn);
                Parallel.ForEach(allExistedFiles, new ParallelOptions() { MaxDegreeOfParallelism = threads },
                file =>
                {
                    if (cancelSearch == true)
                    {
                        return;
                    }
                    UpdateStatus(file);

                    var dirToTouch = Path.GetDirectoryName(file);
                    if (touchedDirs.Contains(dirToTouch) == false)
                    {
                        touchedDirs.Add(dirToTouch);
                        var foundByPattern = Directory.EnumerateFiles(dirToTouch, searchFor).ToList();

                        Dictionary<string, List<DirectoryInfo>> touched = new Dictionary<string, List<DirectoryInfo>>();
                        foreach (var fileItem in foundByPattern)
                        {
                            var fileName = Path.GetFileName(fileItem);
                            var directoryOfFile = new FileInfo(fileItem).Directory;

                            List<DirectoryInfo> parents = null;
                            if (touched.ContainsKey(directoryOfFile.FullName))
                            {
                                parents = touched[directoryOfFile.FullName];
                            }
                            else
                            {
                                parents = GetParentDirectoriesTreeList(directoryOfFile);
                                parents.Reverse();
                            }

                            var fileIconKey = Path.GetExtension(fileItem);
                            if (fileIconKey == ".exe")
                            {
                                fileIconKey = fileItem;
                            }
                            var icon = Icon.ExtractAssociatedIcon(fileItem).ToBitmap();

                            var foundinTree = FindNodeInTreeByKey(parents.First().FullName);
                            if (foundinTree != null)
                            {
                                UpdateIcons(fileIconKey, icon);
                                AddNodeToTree(foundinTree, fileItem, fileName, fileIconKey);
                            }
                            else
                            {
                                int i = 1;
                                while (foundinTree == null)
                                {
                                    foundinTree = FindNodeInTreeByKey(parents[i].FullName);
                                    i++;
                                }

                                TreeNode currentNode = foundinTree;

                                for (int j = i - 2; j >= 0; j--)
                                {
                                    var newNode = AddNodeToTree(currentNode, parents[j].FullName, parents[j].Name, fodlerIconKey);
                                    currentNode = newNode;
                                }

                                UpdateIcons(fileIconKey, icon);
                                AddNodeToTree(currentNode, fileItem, fileName, fileIconKey);
                            }
                        }
                    }
                });

                foreach (var item in Directory.GetDirectories(searchIn))
                {
                    if (cancelSearch == true)
                    {
                        return;
                    }
                    Search(item, searchFor, threads);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
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

        private TreeNode AddNodeToTree(TreeNode targetNode, string key, string value, string imageKey = "")
        {
            TreeNode addedNode = null;
            void add ()
            {
                targetNode.ExpandAll();
                if (string.IsNullOrEmpty(imageKey))
                {
                    addedNode = targetNode.Nodes.Add(key, value);
                }
                else
                {
                    addedNode = targetNode.Nodes.Add(key, value, imageKey, imageKey);
                }

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

        private List<DirectoryInfo> GetParentDirectoriesTreeList(DirectoryInfo directoryInfo)
        {
            DirectoryInfo root = directoryInfo.Root;
            DirectoryInfo parent = directoryInfo;
            var parents = new List<DirectoryInfo>() { parent };
            while (parent.FullName != root.FullName)
            {
                parent = parent.Parent;
                parents.Add(parent);
            }

            parents.Reverse();
            return parents;
        }
    }
}
