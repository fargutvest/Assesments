using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Assesment
{
    public partial class Form : System.Windows.Forms.Form
    {
        private Searcher searcher;

        private TreeNode rootTreeNode;

        public Form()
        {
            InitializeComponent();

            treeView1.ImageList = new ImageList();
            treeView1.ImageList.ColorDepth = ColorDepth.Depth32Bit;

            searcher = new Searcher();
            searcher.TreeRootInited += Searcher_TreeRootInited;
            searcher.Message += Searcher_Message;
            searcher.Progress += Searcher_Progress;
            searcher.IconAdded += Searcher_IconAdded;
            searcher.AddedNodeToTree += Searcher_AddedNodeToTree;
        }

        private void Searcher_TreeRootInited(NodeModel rootNode)
        {
            Invoke((Action)(() =>
            {
                rootTreeNode = new TreeNode()
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
                    recurse(rootTreeNode, subNode);
                }

                treeView1.Nodes.Clear();
                rootTreeNode.ExpandAll();
                treeView1.Nodes.Add(rootTreeNode);
            }));
        }

        private void Searcher_AddedNodeToTree(NodeModel addedNode)
        {
            Invoke((Action)(() =>
            {
                if (rootTreeNode != null)
                {
                    TreeNode targetTreeNode;
                    if (rootTreeNode.Name == addedNode.Parent.Name)
                    {
                        targetTreeNode = rootTreeNode;
                    }
                    else
                    {
                        targetTreeNode = rootTreeNode.Nodes.Find(addedNode.Parent.Name, searchAllChildren: true).First();
                    }

                    targetTreeNode.ExpandAll();
                    var addedTreeNode = targetTreeNode.Nodes.Add(addedNode.Name, addedNode.Text, addedNode.ImageKey, addedNode.SelectedImageKey);
                    targetTreeNode.ExpandAll();
                    addedTreeNode.ExpandAll();
                }
            }));
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
                status.Text = message;
            }));
        }

        private void Searcher_Message(string message)
        {
            MessageBox.Show(message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            var searchFor = this.searchFor.Text;
            searchFor = string.IsNullOrEmpty(searchFor) ? "*.*" : searchFor;
            var searchIn = this.searchIn.Text;
            var threads = (int)countOfThreads.Value;

            treeView1.Nodes.Clear();
            searcher.Start(searchFor, searchIn, threads);
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            searcher.Cancel();
        }
    }
}
