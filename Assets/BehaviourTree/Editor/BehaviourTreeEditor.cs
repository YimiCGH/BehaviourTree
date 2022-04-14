using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;
using BT;
using BT.Util;
using UnityEditor.UIElements;

public class BehaviourTreeEditor : EditorWindow
{
    BehaviourTreeView treeView;
    InspectorView inspectorView;
    BehaviourTreeBlackboardView blackboardView;

    SerializedObject treeObject;
    SerializedProperty blackboardProperty;

    BehaviourTree CurEditrTree;

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
        VisualElement root = rootVisualElement;
   
        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/BehaviourTree/Editor/BehaviourTreeEditor.uxml");
        visualTree.CloneTree(root);

        // 加载样式表，样式会应用到这个 VisualElement 以及它所有的子对象中
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/BehaviourTree/Editor/BehaviourTreeEditor.uss");   
        root.styleSheets.Add(styleSheet);

        treeView = root.Q<BehaviourTreeView>();
        inspectorView = root.Q<InspectorView>();

        BindBlackboard(root);

        BindToolbarBtn(root);

        treeView.OnNodeSelected = OnNodeSelectionChange;
        OnSelectionChange();
    }

    void BindBlackboard(VisualElement root) {
        blackboardView = new BehaviourTreeBlackboardView(root, treeView);
        blackboardView.visible = false;
    }

    void BindToolbarBtn(VisualElement root)
    {
        var toolbar = root.Q<Toolbar>();
        
        var create = new ToolbarButton{name = "create-btn",text = "创建行为树"};
        create.clicked += OnClickCreate;
        toolbar.Add(create);
        
        var open = new ToolbarButton{name = "open-btn",text = "打开行为树"};
        open.clicked += OnClickOpen;
        toolbar.Add(open);
        
        var savelua = new ToolbarButton{name = "savelua-btn",text = "导出Lua"};
        savelua.clicked += OnClickSaveLua;
        toolbar.Add(savelua);
        
        var newnode =new ToolbarButton{name = "newnode-btn",text = "新建节点"};
        newnode.clicked += OnClickNewNode;
        toolbar.Add(newnode);

        var toggleBlackBoard = new ToolbarToggle();
        toggleBlackBoard.text = "黑板";
        toggleBlackBoard.RegisterCallback<MouseDownEvent>(_ =>
        {
            blackboardView.visible = !toggleBlackBoard.value;
        },TrickleDown.TrickleDown);
        toolbar.Add(toggleBlackBoard);
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
            CurEditrTree = tree;
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
        path = PathUtil.ToPrjectPath(path);

        var bt = ScriptableObject.CreateInstance<BehaviourTree>();
        AssetDatabase.CreateAsset(bt, path);
        Selection.activeObject = bt;
        AssetDatabase.Refresh();
    }
    void OnClickOpen() {
        var path = EditorUtility.OpenFilePanel("打开行为树", "Assets/BehaviourTree/BT", "asset");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        path = PathUtil.ToPrjectPath(path);
        var asset = AssetDatabase.LoadAssetAtPath<BehaviourTree>(path);
        if (asset == null) {
            Debug.LogError("打开失败,"+path);
            return;
        }
        AssetDatabase.OpenAsset(asset);
    }

    void OnClickSaveLua()
    {
        var path = EditorUtility.SaveFilePanel("导出lua", "DevTool/LuaScripts/BehaviourTree/Trees/", "BT_New", "lua");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        LuaSaveHelper.ExportLua(CurEditrTree, path);
    }

    void OnClickNewNode()
    {
        NewNodeWindow.Open();
    }
}