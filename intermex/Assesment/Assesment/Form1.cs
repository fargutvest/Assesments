using System;
using System.Drawing;
using System.Windows.Forms;

namespace Assesment
{
    public partial class Form1 : Form
    {
        private Searcher searcher;

        public Form1()
        {
            InitializeComponent();
            searcher = new Searcher();
            searcher.TreeRootInited += Searcher_TreeRootInited;
            searcher.Message += Searcher_Message;
            searcher.Progress += Searcher_Progress;
            searcher.IconAdded += Searcher_IconAdded;
            searcher.AddNodeToTree += Searcher_AddNodeToTree;
            searcher.SearchCompleted += Searcher_SearchCompleted;
        }

        private void Searcher_SearchCompleted(string message)
        {
            Invoke((Action)(() =>
            {
                status.Text = message;
            }));
        }

        private void Searcher_TreeRootInited(TreeNode rootNode)
        {
            Invoke((Action)(() =>
            {
                treeView1.Nodes.Clear();
                rootNode.ExpandAll();
                treeView1.Nodes.Add(rootNode);
            }));
        }

        private TreeNode Searcher_AddNodeToTree(TreeNode targetNode, string nodeId, string nodeText, string imageKey)
        {
            TreeNode addedNode = null;
            void add()
            {
                targetNode.ExpandAll();
                addedNode = targetNode.Nodes.Add(nodeId, nodeText, imageKey, imageKey);
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

        private void Searcher_IconAdded(string key, Bitmap icon)
        {
            Invoke((Action)(() =>
            {
                if (treeView1.ImageList == null)
                {
                    treeView1.ImageList = new ImageList();
                    treeView1.ImageList.ColorDepth = ColorDepth.Depth32Bit;
                }

                treeView1.ImageList.Images.Add(key, icon);
            }));
        }

        private void Searcher_Progress(string message)
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
