using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BT
{
    public class ValueNodeView: NodeView
    {
        private ValueNode _valueNode;
   
        protected override void InitInputPorts()
        {
            CreateInputPort("", Port.Capacity.Single, typeof(string));
        }

        protected override void InitOutputPorts()
        {
        }

        protected override void OnInit()
        {
            _valueNode = Node as ValueNode;
            var Node_SerializedObj = new SerializedObject(_valueNode);
            TextField nameField = new TextField();
            nameField.Bind(Node_SerializedObj);
            nameField.bindingPath = "ValueName";
            nameField.SetValueWithoutNotify(_valueNode.ValueName);
            mainContainer.Add(nameField);
        }
    }
}