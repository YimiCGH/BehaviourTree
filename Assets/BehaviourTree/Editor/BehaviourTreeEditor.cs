using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;
using BT;
using System;
using UnityEditor.UIElements;

public class BehaviourTreeEditor : EditorWindow
{
    BehaviourTreeView treeView;
    InspectorView inspectorView;
    IMGUIContainer blackboardView;

    SerializedObject treeObject;
    SerializedProperty blackboardProperty;

    [MenuItem("BehaviourTreeEditor/Open Editor")]
    public static void OpenEditor()
    {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditor");
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId,int line) {
        if (Selection.activeObject is BehaviourTree) {
            OpenEditor();
            return true;
        }
        return false;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

   
        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/BehaviourTree/Editor/BehaviourTreeEditor.uxml");
        visualTree.CloneTree(root);        

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/BehaviourTree/Editor/BehaviourTreeEditor.uss");   
        root.styleSheets.Add(styleSheet);

        treeView = root.Q<BehaviourTreeView>();
        inspectorView = root.Q<InspectorView>();

        blackboardView = root.Q<IMGUIContainer>();
        blackboardView.onGUIHandler = () =>
        {
            if (treeObject != null && treeObject.targetObject != null) {
                int id = treeObject.targetObject.GetInstanceID();
                
                treeObject.Update();
                EditorGUILayout.PropertyField(blackboardProperty);
                treeObject.ApplyModifiedProperties();
            }            
        };

        treeView.OnNodeSelected = OnNodeSelectionChange;

        var create = root.Q<ToolbarButton>("create-btn");
        create.clicked += OnClickCreate;
        OnSelectionChange();
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

       
    }
    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }
    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingEditMode:
                break;
            case PlayModeStateChange.EnteredPlayMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingPlayMode:
                break;
            default:
                break;
        }
    }

    

    private void OnSelectionChange()
    {
        BehaviourTree tree = Selection.activeObject as BehaviourTree;

        if (tree == null) {
            if (Selection.activeGameObject) {
                var behaviourTreeRunner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
                if (behaviourTreeRunner) {
                    tree = behaviourTreeRunner.GetRunningBT;
                }
            }
        }

        if (Application.isPlaying)
        {
            if (tree && treeView != null)
            {
                treeView.UpdateTreeView(tree);
            }
        }
        else {
            if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
            {
                treeView.UpdateTreeView(tree);
            }
        }

        if (tree != null) {
            treeObject = new SerializedObject(tree);
            blackboardProperty = treeObject.FindProperty("Blackboard");
        }
    }

    void OnNodeSelectionChange(NodeView node) {
        inspectorView.UpdateSelection(node);
    }

    private void OnInspectorUpdate()
    {
        if (!Application.isPlaying) {
            return;
        }
        if (treeView != null) {
            treeView.UpdateNodeStates();
        }
    }

    void OnClickCreate() {
        var path =  EditorUtility.SaveFilePanel("创建行为树","Assets/BehaviourTree/BT","BT_New","asset");
        if (string.IsNullOrEmpty(path)) {
            return;
        }
        path = ToPrjectPath(path);

        var bt = ScriptableObject.CreateInstance<BehaviourTree>();
        AssetDatabase.CreateAsset(bt, path);
        Selection.activeObject = bt;
        AssetDatabase.Refresh();
    }


    static string ToPrjectPath(string _path) {
        var dataPath = Application.dataPath;
        return "Assets" + _path.Remove(0,dataPath.Length);
    }
}