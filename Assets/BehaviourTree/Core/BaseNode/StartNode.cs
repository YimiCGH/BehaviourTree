using UnityEngine;

namespace BT
{
    public class StartNode : BTNode
    {
        public BTNode Child;

        protected override void OnStart()
        {
            
        }

        protected override void OnStop()
        {            
        }

        protected override E_State OnUpdate()
        {
            return Child.Update();
        }

        public override BTNode Clone()
        {
            StartNode node = Instantiate(this);
            node.Child = Child.Clone();
            return node;
        }
    }
}