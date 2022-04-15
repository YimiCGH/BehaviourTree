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
            var textField = new TextField("");
            textField.RegisterValueChangedCallback(etv =>
            {
                _logNode.Message = etv.newValue;
            });
            textField.SetValueWithoutNotify(_logNode.Message);
            mainContainer.Add(textField);
        }
    }
}