using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    [NodeView("BT.CompositeNodeView")]
    public abstract class CompositeNode : BTNode
    {
        public string PreRun;
        public string EndRun;
        public List<BTNode> Children = new List<BTNode>();

        public virtual void AddChild(BTNode _node)
        {
            Children.Add(_node);
        }

        public virtual void RemoveChild(BTNode _node)
        {
            Children.Remove(_node);
        }

        public override BTNode Clone()
        {
            CompositeNode node = Instantiate(this);
            node.Children = Children.ConvertAll(c => c.Clone());
            //相当于下面语句
            /*
            foreach (var child in Children)
            {
                node.Children.Add(child.Clone());
            }
           */
            return node;
        }
    }
}