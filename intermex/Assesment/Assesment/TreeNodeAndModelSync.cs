using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Assesment
{
    internal class TreeNodeAndModelSync
    {
        private Control dispatcher;

        public TreeNodeAndModelSync(Control dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public TreeNode CreateTreeNode(NodeModel rootModel)
        {
            var rootTree = new TreeNode()
            {
                Name = rootModel.Name,
                Text = rootModel.Text,
                ImageKey = rootModel.ImageKey,
                SelectedImageKey = rootModel.SelectedImageKey
            };

            void recursion(TreeNode treeNode, NodeModel model)
            {
                var addedTreeNode = new TreeNode()
                {
                    Text = model.Text,
                    Name = model.Name,
                    SelectedImageKey = model.SelectedImageKey,
                    ImageKey = model.ImageKey
                };

                treeNode.Nodes.Add(addedTreeNode);

                foreach (var subNode in model.Nodes)
                {
                    recursion(addedTreeNode, subNode);
                }
            }

            foreach (var subNode in rootModel.Nodes)
            {
                recursion(rootTree, subNode);
            }

            return rootTree;
        }

        private TreeNode EnsureTargetTreeNodeExists(TreeNode rootTree, NodeModel addedModel)
        {
            TreeNode targetNode;
            if (rootTree.Name == addedModel.Parent.Name)
            {
                targetNode = rootTree;
            }

            targetNode = rootTree.Nodes.Find(addedModel.Parent.Name, searchAllChildren: true).FirstOrDefault();

            if (targetNode == null)
            {
                var parentsUntilRoot = new List<NodeModel>();

                var parent = addedModel.Parent;
                while (parent != null)
                {
                    parentsUntilRoot.Add(parent);
                    parent = parent.Parent;
                }

                int indexOfFoundParent = 0;

                while (targetNode == null)
                {
                    if (rootTree.Name == parentsUntilRoot[indexOfFoundParent].Name)
                    {
                        targetNode = rootTree;

                    }
                    else
                    {
                        targetNode = rootTree.Nodes.Find(parentsUntilRoot[indexOfFoundParent].Name, searchAllChildren: true).FirstOrDefault();
                        indexOfFoundParent++;
                    }
                }
                indexOfFoundParent--;

                OnSafeAndDispatch(() =>
                {
                    for (var i = indexOfFoundParent - 1; i >= 0; i--)
                    {
                        var parentModel = parentsUntilRoot[i];
                        targetNode = targetNode.Nodes.Add(parentModel.Name, parentModel.Text, parentModel.ImageKey, parentModel.SelectedImageKey);
                    }

                    targetNode = targetNode.Nodes.Add(addedModel.Parent.Name, addedModel.Parent.Text, addedModel.Parent.ImageKey, addedModel.Parent.SelectedImageKey);
                });
            }

            return targetNode;
        }

        public void AddNodeToTree(TreeNode rootTree, NodeModel modelToAdd)
        {
            if (rootTree != null)
            {
                TreeNode targetTreeNode;
                if (rootTree.Name == modelToAdd.Parent.Name)
                {
                    targetTreeNode = rootTree;
                }
                else
                {
                    targetTreeNode = EnsureTargetTreeNodeExists(rootTree, modelToAdd);
                }
                OnSafeAndDispatch(() =>
                {
                    var addedTreeNode = targetTreeNode.Nodes.Add(modelToAdd.Name, modelToAdd.Text, modelToAdd.ImageKey, modelToAdd.SelectedImageKey);
                });
            }
        }

        private void OnSafeAndDispatch(Action toDo)
        {
            try
            {
                dispatcher.Invoke(toDo);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
