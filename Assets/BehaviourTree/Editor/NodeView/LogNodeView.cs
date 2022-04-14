using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
namespace BT
{
    public class LogNodeView: NodeView
    {
        private LogNode _logNode;
        public LogNodeView(BTNode node):base(node)
        {
            _logNode = node as LogNode;
            Init();
        }

        protected override void InitOutputPorts()
        {
        }

        void Init()
        {
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