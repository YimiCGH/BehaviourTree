using System.Collections.Generic;

namespace BT
{
    public abstract class FunctionNode:CompositeNode
    {
        public override BTNode Clone()
        {
            FunctionNode node = Instantiate(this);
            node.Children = Children.ConvertAll(c => c.Clone());
            return node;
        }
    }
}