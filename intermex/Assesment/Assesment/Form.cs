using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Assesment
{
    public partial class Form : System.Windows.Forms.Form
    {
        private Searcher searcher;
        private TreeNode rootOfTree;
        private TreeNode selectedTreeNode;
        private object syncRootTree = new object();
        private TreeNodeAndModelSync treeNodeSync;
        private string searchForCacheFilePath = "searchfor.cache";
        private string searchInCacheFilePath = "searchin.cache";
        private string countOfThreadsCacheFilePath = "countOfThreads.cache";
        private bool isStarted = false;

        public Form()
        {
            InitializeComponent();
        }
        private void Form_Load(object sender, EventArgs e)
        {
            LoadUserInputCache();

            treeNodeSync = new TreeNodeAndModelSync(this);
            treeView1.ImageList = new ImageList();
            treeView1.ImageList.ColorDepth = ColorDepth.Depth32Bit;

            searcher = new Searcher();
            searcher.TreeCreated += Searcher_TreeCreated;
            searcher.AddedNodeToTree += Searcher_AddedNodeToTree;
            searcher.Message += Searcher_Message;
            searcher.Progress += Searcher_Progress;
            searcher.Finished += Searcher_Finished;
            searcher.IconAdded += Searcher_IconAdded;

            countOfThreadsNud.Maximum = Environment.ProcessorCount;
            var contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add("Copy", (_, __) =>
            {
                Clipboard.SetText(statusLb.Text);
            });
            statusLb.ContextMenu = contextMenu;
        }

        private void Searcher_Finished(string message)
        {
            isStarted = false;
            OnSafeAndDispatch(() =>
            {
                if (message.Contains(Environment.NewLine))
                {
                    message = message.Split('\n')[0];
                }
                summaryLb.Text = message;
                statusLb.Text = message;
            });
        }

        private void Searcher_TreeCreated(NodeModel rootModel)
        {
            rootOfTree = treeNodeSync.CreateTreeNode(rootModel);
            OnSafeAndDispatch(() =>
            {
                var scrollPos = treeView1.GetScrollPos();
                treeView1.Nodes.Clear();
                rootOfTree.ExpandAll();
                treeView1.Nodes.Add(rootOfTree);
                treeView1.SetScrollPos(scrollPos);
            });
        }

        private void Searcher_AddedNodeToTree(NodeModel addedModel)
        {
            lock (syncRootTree)
            {
                if (rootOfTree != null)
                {
                    var scrollPos = default(Point);
                    OnSafeAndDispatch(() =>
                    {
                        scrollPos = treeView1.GetScrollPos();
                    });
                    treeNodeSync.AddNodeToTree(rootOfTree, addedModel);
                    OnSafeAndDispatch(() =>
                    {
                        rootOfTree.ExpandAll();
                        treeView1.SetScrollPos(scrollPos);
                    });
                }
            }
        }

        private void Searcher_IconAdded(string key, Bitmap icon)
        {
            OnSafeAndDispatch(() =>
            {
                treeView1.ImageList.Images.Add(key, icon);
            });
        }

        private void Searcher_Progress(string message)
        {
            OnSafeAndDispatch(() =>
            {
                statusLb.Text = message;
            });
        }

        private void Searcher_Message(string message)
        {
            MessageBox.Show(message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            if (isStarted)
            {
                return;
            }

            isStarted = true;

            var searchFor = this.searchForCb.Text;
            var searchIn = this.searchInCb.Text;
            var threads = (int)countOfThreadsNud.Value;
            selectedTreeNode = null;
            goToFileBtn.Enabled = false;
            summaryLb.Text = "";

            treeView1.Nodes.Clear();
            rootOfTree = null;

            if (searchInCb.Items.Contains(searchIn))
            {
                searchInCb.Items.Remove(searchIn);
            }
            searchInCb.Items.Insert(0, searchIn);
            searchInCb.SelectedIndex = 0;
            File.WriteAllLines(searchInCacheFilePath, searchInCb.Items.Cast<string>());

            if (searchForCb.Items.Contains(searchFor))
            {
                searchForCb.Items.Remove(searchFor);
            }
            searchForCb.Items.Insert(0, searchFor);
            searchForCb.SelectedIndex = 0;
            File.WriteAllLines(searchForCacheFilePath, searchForCb.Items.Cast<string>());

            File.WriteAllText(countOfThreadsCacheFilePath, threads.ToString());

            searcher.Start(searchFor, searchIn, threads);
        }


        private void LoadUserInputCache()
        {
            if (File.Exists(searchForCacheFilePath))
            {
                searchForCb.Items.Clear();
                searchForCb.Items.AddRange(File.ReadAllLines(searchForCacheFilePath));
            }

            if (File.Exists(searchInCacheFilePath))
            {
                searchInCb.Items.Clear();
                searchInCb.Items.AddRange(File.ReadAllLines(searchInCacheFilePath));
            }
            if (searchInCb.Items.Count > 0)
            {
                searchInCb.SelectedIndex = 0;
            }

            if (File.Exists(countOfThreadsCacheFilePath))
            {
                int.TryParse(File.ReadAllText(countOfThreadsCacheFilePath), out var countOfThreads);
                countOfThreads = countOfThreads == 0 ? 1 : countOfThreads;
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
                    if (new DirectoryInfo(selectedTreeNode.Name).Root.FullName == selectedTreeNode.Name == false)
                    {
                        Process.Start(Path.GetDirectoryName(selectedTreeNode.Name));
                    }
                }
            }
        }

        private void OnSafeAndDispatch(Action toDo)
        {
            try
            {
                Invoke(toDo);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
