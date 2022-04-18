using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BT
{
    public class FunctionNodeView: NodeView
    {
        private FunctionNode _functionNode;
     

        protected override void InitOutputPorts()
        {
        }

        protected override void OnInit()
        {
            _functionNode = Node as FunctionNode;
            
            var type = _functionNode.GetType();
            foreach (var field in type.GetFields())
            {
                var attr = field.GetCustomAttribute<FunctionParamsAttribute>();
                if (attr != null)
                {
                    CreateOutputPort(attr.ParamName, Port.Capacity.Single, typeof(string));
                }
            }
        }
    }
}