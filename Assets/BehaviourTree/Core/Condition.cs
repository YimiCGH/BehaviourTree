using System.Collections;
using System.Text;
using UnityEngine;
using Sirenix.OdinInspector;
namespace BT
{
    [System.Serializable]
    public class Condition
    {
        public enum E_ValueType {
            [LabelText("常量")]
            Const,
            [LabelText("自身变量")]
            SelfVar,
            [LabelText("黑板变量")]
            Blackboard
        }
        
        [HorizontalGroup("变量1"),HideLabel]
        public E_ValueType Value1_Type;

        [HorizontalGroup("变量名1"), HideLabel]
        public string Value1_Name;

        [HorizontalGroup("比较符"),ValueDropdown("GetCompareString"), HideLabel]
        public string CompareType = "==";

        [HorizontalGroup("变量2"), HideLabel]
        public E_ValueType Value2_Type;
        [HorizontalGroup("变量名2"), HideLabel]
        public string Value2_Name;


        IEnumerable GetCompareString = new ValueDropdownList<string>() {
            {"==","=="},
            {"!=","~="},
            {">=",">="},
            {"<=","<="},
            {">",">"},
            {"<","<"},
        };

        public string ToLuaString()
        {
            StringBuilder sb = new StringBuilder();

            string v1_name = Value1_Type == E_ValueType.Const ? Value1_Name : $"'{Value1_Name}'";
            string v2_name = Value2_Type == E_ValueType.Const ? Value2_Name : $"'{Value2_Name}'";
            sb.Append($"{{value1_type = {(int)Value1_Type} ,value1_name = {v1_name} ,compareType = '{CompareType}' ,value2_type = {(int)Value2_Type} ,value2_name = {v2_name} }}");
            return sb.ToString();
        }
    }
}