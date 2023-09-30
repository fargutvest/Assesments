using System.Collections.Generic;

namespace Assesment
{
    internal class NodeModel
    {
        public NodeModel Parent { get; set; }
        public List<NodeModel> Nodes { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public string ImageKey { get; set; }
        public string SelectedImageKey { get; set; }

        public NodeModel()
        {
            Nodes = new List<NodeModel>();
        }
    }
}
