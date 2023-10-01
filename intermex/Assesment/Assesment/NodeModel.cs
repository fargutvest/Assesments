using System.Collections.Concurrent;

namespace Assesment
{
    internal class NodeModel
    {
        public NodeModel Parent { get; set; }
        public ConcurrentBag<NodeModel> Nodes { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public string ImageKey { get; set; }
        public string SelectedImageKey { get; set; }

        public bool IsFile { get; set; }

        public NodeModel()
        {
            Nodes = new ConcurrentBag<NodeModel>();
        }

        public override string ToString()
        {
            if (IsFile)
            {
                return $"{Name}";
            }
            else
            {
                return $"{Name} [{Nodes.Count}]";
            }
        }
    }
}
