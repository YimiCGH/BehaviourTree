using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public abstract class CompositeNode : BTNode
    {
        public List<BTNode> Children = new List<BTNode>();

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