using UnityEngine;

namespace BT
{
    public abstract class DecoratorNode : BTNode
    {
        public BTNode Child;
        public override BTNode Clone()
        {
            DecoratorNode node = Instantiate(this);
            node.Child = Child.Clone();
            return node;
        }
    }
}