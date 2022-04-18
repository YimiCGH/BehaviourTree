using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BT
{
    public class CompareNodeView: NodeView
    {
        private CompareNode _compareNode;
   
        protected override void InitOutputPorts()
        {
            CreateOutputPort("A", Port.Capacity.Single, typeof(string));
            CreateOutputPort("B", Port.Capacity.Single, typeof(string));
        }
        protected override void OnInit()
        {
            _compareNode = Node as CompareNode;
            
            VisualElement container = new VisualElement();

            var leftLabel = new Label("A"); 
            container.Add(leftLabel);
            
            var Node_SerializedObj = new SerializedObject(_compareNode);
            TextField compareField = new TextField();
            compareField.Bind(Node_SerializedObj);
            compareField.bindingPath = "CompareType";
            compareField.SetValueWithoutNotify(_compareNode.CompareType);
            container.Add(compareField);
            
            var rightLabel = new Label("B"); 
            container.Add(rightLabel);

            container.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            container.style.justifyContent = new StyleEnum<Justify>(Justify.Center);
            
            mainContainer.Add(container);
        }
    }
}