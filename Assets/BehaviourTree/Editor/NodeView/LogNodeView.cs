using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
namespace BT
{
    public class LogNodeView: NodeView
    {
        private LogNode _logNode;
      
        protected override void InitOutputPorts()
        {
        }

        protected override void OnInit()
        {
            _logNode = Node as LogNode;
            var Node_SerializedObj = new SerializedObject(_logNode);
            
            var textField = new TextField("");
            textField.Bind(Node_SerializedObj);
            textField.bindingPath = "Message";
            mainContainer.Add(textField);
        }
    }
}