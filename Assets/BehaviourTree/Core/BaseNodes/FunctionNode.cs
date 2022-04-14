using System.Collections.Generic;

namespace BT
{
    public abstract class FunctionNode:BTNode
    {
        public string FunctionName;
        
        public List<BTNode> Children = new List<BTNode>();
        public override BTNode Clone()
        {
            FunctionNode node = Instantiate(this);
            node.Children = Children.ConvertAll(c => c.Clone());
            return node;
        }
    }
}