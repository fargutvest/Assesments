using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Assesment
{
    public partial class Form : System.Windows.Forms.Form
    {
        private Searcher searcher;

        private TreeNode rootOfTree;

        private TreeNode selectedTreeNode;

        private object syncRootTree = new object();

        public Form()
        {
            InitializeComponent();

            treeView1.ImageList = new ImageList();
            treeView1.ImageList.ColorDepth = ColorDepth.Depth32Bit;

            searcher = new Searcher();
            searcher.TreeCreated += Searcher_TreeCreated;
            searcher.Message += Searcher_Message;
            searcher.Progress += Searcher_Progress;
            searcher.Finished += Searcher_Finished; ;
            searcher.IconAdded += Searcher_IconAdded;
            searcher.AddedNodeToTree += Searcher_AddedNodeToTree;

            countOfThreadsNud.Maximum = Environment.ProcessorCount;
            var mnuContext = new ContextMenu();
            mnuContext.MenuItems.Add("Copy", (sender, e) => 
            {
                Clipboard.SetText(statusLb.Text);
            });

            statusLb.ContextMenu = mnuContext;
        }

        private void Searcher_Finished(string message)
        {
            Invoke((Action)(() =>
            {
                summaryLb.Text = message;
            }));
        }

        private void Searcher_TreeCreated(NodeModel rootNode)
        {
            lock (syncRootTree)
            {
                Invoke((Action)(() =>
            {
                rootOfTree = new TreeNode()
                {
                    Text = rootNode.Text,
                    Name = rootNode.Name,
                    SelectedImageKey = rootNode.SelectedImageKey,
                    ImageKey = rootNode.ImageKey
                };

                void recurse(TreeNode treeNode, NodeModel node)
                {
                    var addedTreeNode = new TreeNode()
                    {
                        Text = node.Text,
                        Name = node.Name,
                        SelectedImageKey = node.SelectedImageKey,
                        ImageKey = node.ImageKey
                    };

                    treeNode.Nodes.Add(addedTreeNode);

                    foreach (var subNode in node.Nodes)
                    {
                        recurse(addedTreeNode, subNode);
                    }
                }

                foreach (var subNode in rootNode.Nodes)
                {
                    recurse(rootOfTree, subNode);
                }

                treeView1.Nodes.Clear();
                rootOfTree.ExpandAll();
                treeView1.Nodes.Add(rootOfTree);
            }));
            }
        }

        private void Searcher_AddedNodeToTree(NodeModel addedNode)
        {
            lock (syncRootTree)
            {
                if (rootOfTree != null)
                {
                    TreeNode targetTreeNode;
                    if (rootOfTree.Name == addedNode.Parent.Name)
                    {
                        targetTreeNode = rootOfTree;
                    }
                    else
                    {
                        targetTreeNode = EnsureTargetTreeNode(addedNode);
                    }

                    Invoke((Action)(() =>
                    {
                        targetTreeNode.ExpandAll();
                        var addedTreeNode = targetTreeNode.Nodes.Add(addedNode.Name, addedNode.Text, addedNode.ImageKey, addedNode.SelectedImageKey);
                        targetTreeNode.ExpandAll();
                        addedTreeNode.ExpandAll();
                    }));
                }
            }
        }

        private TreeNode EnsureTargetTreeNode(NodeModel addedNode)
        {
            TreeNode targetNode;
            if (rootOfTree.Name == addedNode.Parent.Name)
            {
                targetNode = rootOfTree;
            }

            targetNode = rootOfTree.Nodes.Find(addedNode.Parent.Name, searchAllChildren: true).FirstOrDefault();

            if (targetNode == null)
            {
                List<NodeModel> parentsUntilRoot = new List<NodeModel>();

                var parent = addedNode.Parent;
                while (parent != null)
                {
                    parentsUntilRoot.Add(parent);
                    parent = parent.Parent;
                }

                int indexOfFoundParent = 0;

                while (targetNode == null)
                {
                    if (rootOfTree.Name == parentsUntilRoot[indexOfFoundParent].Name)
                    {
                        targetNode = rootOfTree;

                    }
                    else
                    {
                        targetNode = rootOfTree.Nodes.Find(parentsUntilRoot[indexOfFoundParent].Name, searchAllChildren: true).FirstOrDefault();
                        indexOfFoundParent++;
                    }
                }
                indexOfFoundParent--;
                Invoke((Action)(() =>
                {
                    for (var i = indexOfFoundParent - 1; i >= 0; i--)
                    {
                        targetNode = targetNode.Nodes.Add(parentsUntilRoot[i].Name, parentsUntilRoot[i].Text, parentsUntilRoot[i].ImageKey, parentsUntilRoot[i].SelectedImageKey);
                    }

                    targetNode = targetNode.Nodes.Add(addedNode.Parent.Name, addedNode.Parent.Text, addedNode.Parent.ImageKey, addedNode.Parent.SelectedImageKey);
                }));
            }

            return targetNode;
        }

        private void Searcher_IconAdded(string key, Bitmap icon)
        {
            Invoke((Action)(() =>
            {
                treeView1.ImageList.Images.Add(key, icon);
            }));
        }

        private void Searcher_Progress(string message)
        {
            Invoke((Action)(() =>
            {
                statusLb.Text = message;
            }));
        }

        private void Searcher_Message(string message)
        {
            MessageBox.Show(message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            var searchFor = this.searchForCb.Text;
            searchFor = string.IsNullOrEmpty(searchFor) ? "*.*" : searchFor;
            var searchIn = this.searchInCb.Text;
            var threads = (int)countOfThreadsNud.Value;
            selectedTreeNode = null;
            goToFileBtn.Enabled = false;

            treeView1.Nodes.Clear();
            rootOfTree = null;

            if (searchInCb.Items.Contains(searchIn) == false)
            {
                searchInCb.Items.Insert(0, searchIn);
                File.WriteAllLines(searchInCacheFilePath, searchInCb.Items.Cast<string>());
            }
            if (searchForCb.Items.Contains(searchFor) == false)
            {
                searchForCb.Items.Insert(0, searchFor);
                File.WriteAllLines(searchForCacheFilePath, searchForCb.Items.Cast<string>());
            }

            File.WriteAllText(countOfThreadsCacheFilePath, threads.ToString());

            searcher.Start(searchFor, searchIn, threads);
        }

        private string searchForCacheFilePath = "searchfor.cache";
        private string searchInCacheFilePath = "searchin.cache";
        private string countOfThreadsCacheFilePath = "countOfThreads.cache";

        private void LoadUserInputCache()
        {
            if (File.Exists(searchForCacheFilePath))
            {
                searchForCb.Items.Clear();
                searchForCb.Items.AddRange(File.ReadAllLines(searchForCacheFilePath));
                if (searchForCb.Items.Count > 0)
                {
                    searchForCb.SelectedIndex = 0;
                }
            }

            if (File.Exists(searchInCacheFilePath))
            {
                searchInCb.Items.Clear();
                searchInCb.Items.AddRange(File.ReadAllLines(searchInCacheFilePath));
                if (searchInCb.Items.Count > 0)
                {
                    searchInCb.SelectedIndex = 0;
                }
            }

            if (File.Exists(countOfThreadsCacheFilePath))
            {
                int.TryParse(File.ReadAllText(countOfThreadsCacheFilePath), out var countOfThreads);
                countOfThreadsNud.Value = countOfThreads > countOfThreadsNud.Maximum ? countOfThreadsNud.Maximum : countOfThreads;
            }
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            searcher.Cancel();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            selectedTreeNode = e.Node;
            goToFileBtn.Enabled = true;
            statusLb.Text = selectedTreeNode.Name;
        }

        private void goToFileBtn_Click(object sender, EventArgs e)
        {
            if (selectedTreeNode != null)
            {
                if (File.Exists(selectedTreeNode.Name))
                {
                    string argument = "/select, \"" + selectedTreeNode.Name + "\"";
                    Process.Start("explorer.exe", argument);
                }
                else
                {
                    Process.Start(Path.GetDirectoryName(selectedTreeNode.Name));
                }
            }
        }

        private void Form_Load(object sender, EventArgs e)
        {
            LoadUserInputCache();
        }
    }
}
