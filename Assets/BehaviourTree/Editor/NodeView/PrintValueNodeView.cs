using UnityEditor.Experimental.GraphView;

namespace BT
{
    public class PrintValueNodeView: NodeView
    {
        protected override void InitOutputPorts()
        {
            CreateOutputPort("", Port.Capacity.Single, typeof(string));
        }
    }
}