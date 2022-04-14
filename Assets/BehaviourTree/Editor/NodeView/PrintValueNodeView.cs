using UnityEditor.Experimental.GraphView;

namespace BT
{
    public class PrintValueNodeView: NodeView
    {
        public PrintValueNodeView(BTNode node) : base(node)
        {
            
        }

        protected override void InitOutputPorts()
        {
            CreateOutputPort("", Port.Capacity.Single, typeof(string));
        }
    }
}