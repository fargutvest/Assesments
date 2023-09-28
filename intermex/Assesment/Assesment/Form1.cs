using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assesment
{
    public partial class Form1 : Form
    {
        ConcurrentBag<string> touchedDirs = new ConcurrentBag<string>();
        ConcurrentDictionary<string, Bitmap> icons = new ConcurrentDictionary<string, Bitmap>();
        TreeNode treeRoot;
        bool cancelSearch = true;
        string fodlerKey = "folder";

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

            touchedDirs = new ConcurrentBag<string>();
            icons = new ConcurrentDictionary<string, Bitmap>();

            cancelSearch = false;
            var searchFor = this.searchFor.Text;
            searchFor = string.IsNullOrEmpty(searchFor) ? "*.*" : searchFor;
            var searchIn = this.searchIn.Text;
            var threads = (int)countOfThreads.Value;

            var icon = (Bitmap)Bitmap.FromFile("icon74.ico");
            UpdateIcons(fodlerKey, icon);

            Task.Run(() =>
            {
                var parents = GetParentDirectoriesTreeList(new DirectoryInfo(searchIn));
                var first = parents.First().FullName;
                treeRoot = new TreeNode(first) { Name = first, ImageKey = fodlerKey, SelectedImageKey = fodlerKey };
                TreeNode currentNode = treeRoot;
                foreach (var item in parents.Skip(1))
                {
                    var newNode = AddNodeToTree(currentNode, item.FullName, item.Name, fodlerKey);
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
                if (cancelSearch == true)
                {
                    Invoke((Action)(() =>
                    {
                        status.Text = "canceled";
                    }));
                }
                else
                {
                    cancelSearch = true;
                    Invoke((Action)(() =>
                    {
                        status.Text = "Complete!";
                    }));
                }
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

                            var foundinTree = FindInTree(parents.First().FullName);
                            if (foundinTree != null)
                            {
                                var key = Path.GetExtension(fileItem);
                                if (key == ".exe")
                                {
                                    key = fileItem;
                                }
                                var icon = Icon.ExtractAssociatedIcon(fileItem).ToBitmap();

                                UpdateIcons(key, icon);
                                AddNodeToTree(foundinTree, fileItem, fileName, key);
                            }
                            else
                            {
                                int i = 1;
                                while (foundinTree == null)
                                {
                                    foundinTree = FindInTree(parents[i].FullName);
                                    i++;
                                }

                                TreeNode currentNode = foundinTree;

                                for (int j = i - 2; j >= 0; j--)
                                {
                                    var newNode = AddNodeToTree(currentNode, parents[j].FullName, parents[j].Name, fodlerKey);
                                    currentNode = newNode;
                                }

                                var key = Path.GetExtension(fileItem);
                                if (key == ".exe")
                                {
                                    key = fileItem;
                                }
                                var icon = Icon.ExtractAssociatedIcon(fileItem).ToBitmap();

                                UpdateIcons(key, icon);
                                AddNodeToTree(currentNode, fileItem, fileName, key);
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
            catch
            {
                Debug.WriteLine(searchIn);
            }

        }

        private void UpdateStatus(string message)
        {
            Invoke((Action)(() =>
            {
                status.Text = message;
            }));
        }

        private void UpdateIcons(string key, Bitmap icon)
        {
            if (icons.ContainsKey(key) == false)
            {
                icons[key] = icon;

                Invoke((Action)(() =>
                {
                    if (treeView1.ImageList == null)
                    {
                        treeView1.ImageList = new ImageList();
                        treeView1.ImageList.ColorDepth = ColorDepth.Depth32Bit;
                    }
                    foreach (var item in icons)
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
            Invoke((Action)(() =>
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
            }));

            return addedNode;
        }

        private TreeNode FindInTree(string key)
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
