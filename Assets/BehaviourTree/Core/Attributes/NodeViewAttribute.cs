using System;

namespace BT
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeViewAttribute:Attribute
    {
        public string ViewName { get; private set; }

        public NodeViewAttribute(string _viewName)
        {
            ViewName = _viewName;
        }
    }
}