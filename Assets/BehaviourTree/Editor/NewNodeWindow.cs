using BT.Util;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
namespace BT
{
    public class NewNodeWindow :OdinEditorWindow
    {
        public enum E_NodeType
        {
            ActionNode,
            DecorateNode,
        }
        [System.Serializable]
        public class NodeScript
        {
            public string NodeName;
            public string CSharpPath;
            public string LuaPath;

            [Button]
            void OpenCSharp()
            {
                
            }

            [Button]
            void OpenLua()
            {
                
            }
        }

        public E_NodeType NodeType;
        public string NodeName;

      
        
        public static void Open()
        {
            var window = EditorWindow.CreateWindow<NewNodeWindow>();
            window.UpdateScripts();
        }

        [Button]
        public void Create()
        {
            
        }

        public string CSharpPath = "Assets/BehaviourTree/Core/CustomNodes/";
        public string LuaPath = "DevTool/LuaScripts/BehaviourTree/Core/CustomNodes/";
        
        [TableList]
        public NodeScript[] NodeScripts;
        void UpdateScripts()
        {
            string csharpPath = PathUtil.GetProjectPath() + CSharpPath;
            string luaPath = PathUtil.GetProjectPath() + LuaPath;
            
            Debug.Log(csharpPath);
            Debug.Log(luaPath);
        }
    }
}