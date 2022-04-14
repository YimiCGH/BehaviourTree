using System;

namespace BT
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FunctionParamsAttribute:Attribute
    {
        public string ParamName { get; set; }

        public FunctionParamsAttribute(string _paramName)
        {
            ParamName = _paramName;
        }
    }
}