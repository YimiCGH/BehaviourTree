using System.Collections.Generic;

namespace BT
{
    [System.Serializable]
    public class NodeGroup
    {
        public string guid;
        public string Title;
        public List<BTNode> Nodes = new List<BTNode>();

        public void AddNode(BTNode _node)
        {
            Nodes.Add(_node);
        }

        public void RemoveNode(BTNode _node)
        {
            Nodes.Remove(_node);
        }
    }
}