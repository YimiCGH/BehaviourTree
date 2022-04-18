using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BT
{
    public class AssignValueNodeView: NodeView
    {
        private AssignValueNode _assignValueNode;
        protected override void InitOutputPorts()
        {
            CreateOutputPort("A", Port.Capacity.Single, typeof(string));
            CreateOutputPort("B", Port.Capacity.Single, typeof(string));
        }

        protected override void OnInit()
        {
            _assignValueNode = Node as AssignValueNode;
            
            VisualElement container = new VisualElement();

            var label = new Label("A = B (将B的值赋给A)"); 
            container.Add(label);

            container.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            container.style.justifyContent = new StyleEnum<Justify>(Justify.Center);
            
            mainContainer.Add(container);
        }
    }
}