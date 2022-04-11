using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
namespace BT
{
    [System.Serializable]
    public class ConditionConfig
    {

        public enum E_ConditionType {
            [LabelText("全部满足")]
            AllFit,
            [LabelText("任意满足")]
            AnyFit,
            [LabelText("全部失败")]
            AllFailure,
            [LabelText("任意失败")]
            AnyFailure
        }
        [LabelText("条件类型")]
        public E_ConditionType ConditionType;
        [TableList]
        public Condition[] Conditions;
    }
}