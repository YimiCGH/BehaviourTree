using System.Collections.Generic;

namespace BT
{
    [Category("组合","按权重随机选取（WeightPickNode）")]
    [NodeView("BT.WeightPickNodeView")]
    public class WeightPickNode:CompositeNode
    {
        public List<int> Weights = new List<int>();

        public override void AddChild(BTNode _node)
        {
            base.AddChild(_node);
            Weights.Add(0);
        }

        public override void RemoveChild(BTNode _node)
        {
            var index = Children.IndexOf(_node);
            Children.RemoveAt(index);
            Weights.RemoveAt(index);
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnStop()
        {
            
        }

        protected override E_State OnUpdate()
        {
            return E_State.Success;
        }
    }
}