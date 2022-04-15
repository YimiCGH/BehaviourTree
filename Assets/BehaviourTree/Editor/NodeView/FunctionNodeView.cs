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
        public FunctionNodeView(BTNode node) : base(node)
        {
            _functionNode = node as FunctionNode;
            Init();
        }

        protected override void InitOutputPorts()
        {
        }

        void Init()
        {
            var type = _functionNode.GetType();
            foreach (var field in type.GetFields())
            {
                var attr = field.GetCustomAttribute<FunctionParamsAttribute>();
                if (attr != null)
                {
                    Debug.Log($"{attr.ParamName}");
                    CreateOutputPort(attr.ParamName, Port.Capacity.Single, typeof(string));
                }
            }
        }
    }
}