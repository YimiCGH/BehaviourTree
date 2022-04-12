using UnityEditor;
using UnityEngine;

namespace BT.Blackboard
{
    public class BlackboardController
    {
        private BehaviourTreeBlackboardView m_blackboard;
        public BlackboardController(BehaviourTreeBlackboardView _bb)
        {
            m_blackboard = _bb;
        }

        public void InitPropertyType(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("String"), false, ()=>m_blackboard.AddField(new BlackboardProperty{PropertyType = "string"}));
            menu.AddItem(new GUIContent("Int"), false,()=>m_blackboard.AddField(new BlackboardProperty{PropertyType = "int"}));
            menu.AddItem(new GUIContent("Float"), false,()=>m_blackboard.AddField(new BlackboardProperty{PropertyType = "float"}));
            menu.AddItem(new GUIContent("Bool"), false,()=>m_blackboard.AddField(new BlackboardProperty{PropertyType = "bool"}));
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("导入黑板"), false,()=>m_blackboard.LoadBlackboard());
            menu.AddItem(new GUIContent("黑板另存为"), false,()=>m_blackboard.SaveBlackboard());
            menu.AddItem(new GUIContent("导出Lua"), false,()=>m_blackboard.ExportToLua());
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("清空黑板"), false,()=>m_blackboard.ClearBlackboard());
        }
    }
}