using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using BT;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class BehaviourTreeView : GraphView
{
    public Action<NodeView> OnNodeSelected;
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> { }

    BehaviourTree _tree;
    public BehaviourTreeView() {
        Insert(0,new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(CreateGroupContextualMenu());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/BehaviourTree/Editor/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);

        elementsAddedToGroup += OnAddElementToGroup;
        elementsRemovedFromGroup += OnRemoveElementToGroup;

        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnRemoveElementToGroup(Group group, IEnumerable<GraphElement> arg2)
    {
        foreach (var element in arg2)
        {
            var node = element as NodeView;
            if (node != null)
            {
                var ngroup = GetNodeGroup(group);
                ngroup.RemoveNode(node.Node);
                Debug.Log($"remove {node.title} from {group.title}");
            }
        }
    }

    private void OnAddElementToGroup(Group group, IEnumerable<GraphElement> arg2)
    {
        foreach (var element in arg2)
        {
            var node = element as NodeView;
            if (node != null)
            {
                var ngroup = GetNodeGroup(group);
                ngroup.RemoveNode(node.Node);
                Debug.Log($"Add {node.title} to {group.title}");
            }
        }
    }

    private void OnUndoRedo()
    {
        UpdateTreeView(_tree);
        AssetDatabase.SaveAssets();
    }

    NodeView FindNodeView(BTNode node) {
        return GetNodeByGuid(node.guid) as NodeView;
    }

    NodeGroup GetNodeGroup(Group group)
    {
        var res = _tree.Groups.Where(ng => ng.guid == group.viewDataKey);
        if (res.Any())
        {
            return res.First();
        }

        return null;
    }

    /// <summary>
    /// 刷新视图
    /// </summary>
    /// <param name="tree"></param>
    internal void UpdateTreeView(BehaviourTree tree)
    {
        _tree = tree;
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        if (_tree.rootNode == null) {
            _tree.rootNode = CreateNode<StartNode>(new Vector2(0,0));
            EditorUtility.SetDirty(_tree);
            AssetDatabase.SaveAssets();
        }

        // Create Nodes
        _tree.Nodes.ForEach(n => CreateNodeView(n));

        // Create Edges
        _tree.Nodes.ForEach( n => {
            var children = _tree.GetChildren(n);
            if (children != null) {
                children.ForEach(c => {
                    var parentView = FindNodeView(n);
                    var childView = FindNodeView(c);
                    CreateEdge(parentView, childView);
                });
            }
        });
        
        //Create Group
        foreach (var ngroup in _tree.Groups)
        {
            AddElement(CreateGroup(ngroup));
        }
       
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(port => 
            port.direction != startPort.direction &&
            port.node != startPort.node &&
            port.portType == startPort.portType
        ).ToList();
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null) {
            graphViewChange.elementsToRemove.ForEach( element => {
                NodeView nodeView = element as NodeView;
                if (nodeView != null) {
                    DeleteNode(nodeView.Node);
                }

                Edge edge = element as Edge;
                if (edge != null) {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    RemoveChild(parentView.Node, childView.Node);
                }
                
                Group group = element as Group;
                if (group != null)
                {
                    var ngroup = GetNodeGroup(group);
                    _tree.Groups.Remove(ngroup);
                    Debug.Log($"remove group {ngroup.Title}");
                }

                
            });
        }

        if (graphViewChange.edgesToCreate != null) {
            graphViewChange.edgesToCreate.ForEach( edge => {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;
                AddChild(parentView.Node,childView.Node);
                parentView.SortChildren();
            });
        }

        if (graphViewChange.movedElements != null) {
            nodes.ForEach((n)=> {
                NodeView view = n as NodeView;
                if (view != null) {
                    view.SortChildren();
                }
            });
        }

        return graphViewChange;
    }

    private Vector2 localMousePosition;
    /// <summary>
    /// 右键菜单命令
    /// </summary>
    /// <param name="evt"></param>

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        localMousePosition = evt.localMousePosition;
        {
            var types = TypeCache.GetTypesDerivedFrom<BTNode>();
            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<CategoryAttribute>();
                if (attr != null)
                {
                    evt.menu.AppendAction($"[{attr.CategoryName}]/{attr.SubName}",(a) => CreateNode(type));    
                }
            }
        }
    }

    private IManipulator CreateGroupContextualMenu()
    {
        ContextualMenuManipulator manipulator = new ContextualMenuManipulator(
            evt =>
            {
                evt.menu.AppendAction("Add Group",
                    actionEvt =>
                    {
                        var group = CreateGroup("Group", actionEvt.eventInfo.localMousePosition);
                        if(group != null)
                            AddElement(group);
                    });
                evt.menu.AppendAction("Remove Group",
                    actionEvt =>
                    {
                        foreach (var obj in selection)
                        {
                            var g = obj as Group;
                            if (g != null)
                            {
                                var ng = GetNodeGroup(g);                                
                                var nodes = ng.Nodes.ConvertAll<NodeView>(n => FindNodeView(n));
                                
                                foreach (var node in nodes)
                                {                                 
                                    g.RemoveElement(node);
                                }
                                RemoveElement(g);
                                _tree.Groups.Remove(ng);
                            }
                            break;
                        }
                        
                    });
            });
        return manipulator;
    }

    GraphElement CreateGroup(NodeGroup ngroup)
    {
        Group group = new Group()
        {
            title = ngroup.Title
        };
        group.viewDataKey = ngroup.guid;
        var title = group.Q<Label>("titleLabel");
        title.RegisterValueChangedCallback(evt => ngroup.Title = evt.newValue);
        foreach (var node in ngroup.Nodes)
        {            
            var n =  FindNodeView(node);
            if (n != null)
            {
                group.AddElement(n);
            }
        }

        return group;
    }

    GraphElement CreateGroup(string groupName,Vector2 position)
    {
        if (selection.Count == 0)
        {
            return null;
        }

        Group group = new Group()
        {
            title = groupName
        };
        group.SetPosition(new Rect(position,Vector2.zero));
        NodeGroup ngroup = new NodeGroup();
        ngroup.guid = group.viewDataKey;
        ngroup.Title = group.title;

        var title = group.Q<Label>("titleLabel");
        title.RegisterValueChangedCallback(evt => ngroup.Title = evt.newValue);
        
        foreach (var obj in selection)
        {
            var n = obj as NodeView;
            if (n != null)
            {
                Debug.Log(n.title);
                group.AddElement(n);
                ngroup.AddNode(n.Node);
            }
        }
        _tree.Groups.Add(ngroup);
        return group;
    }

    void CreateNode(System.Type type) {
        var node = CreateNode(type,localMousePosition);
        CreateNodeView(node);
    }

    void CreateNodeView(BTNode node) {
        //各种节点可以自定义界面
        NodeView nodeView = null;
        var attr = node.GetType().GetCustomAttribute<NodeViewAttribute>();
        if (attr != null)
        {
            var type = Type.GetType(attr.ViewName);//类名需要带命名空间，不然会找不到
            var instance = Activator.CreateInstance(type) ;
            nodeView = instance as NodeView;
        }
        else
        {
            nodeView = new NodeView();
        }
        nodeView.Init(node,this);
        //m_editorWindow.rootVisualElement.ChangeCoordinatesTo(m_editorWindow.rootVisualElement.parent,m_editorWindow.position)

        
        nodeView.OnNodeSelected = OnNodeSelected;
        AddElement(nodeView);
    }
    void CreateEdge(NodeView parent, NodeView child) {
        var edge = parent.ConnectNodeView(child);
        if (edge != null)
        {
            AddElement(edge);
        }
    }

    public void RemovePort(Port port)
    {
        //目前只处理了单选端口的情况，可以多选的端口删除情况还没处理
        var targetEdges = edges.ToList().Where(e => e.output == port);
        if (!targetEdges.Any())
        {
            return;
        }

        var edge = targetEdges.First();
        var parentView = edge.output.node as NodeView;
        var childView = edge.input.node as NodeView;
        RemoveChild(parentView.Node,childView.Node);
        parentView.RemoveOutputPort(port);
        edge.output.Disconnect(edge);
        edge.input.Disconnect(edge);
        RemoveElement(edge);
                
    }

    public void UpdateNodeStates() {
        nodes.ForEach( n => {
            NodeView view = n as NodeView;
            if (view != null) {
                view.UpdateState();
            }
        });
        edges.ForEach( e=> {
            var input_node = e.input.node as NodeView;
            var out_node = e.input.node as NodeView;
            

            if (input_node != null && 
                out_node != null &&
                out_node.Node.Started && 
                out_node.Node.State == E_State.Running)
            {
                e.edgeControl.inputColor = Color.cyan;
                e.edgeControl.outputColor = Color.cyan;
                e.edgeControl.edgeWidth = 4;
            }
            else
            {
                e.edgeControl.inputColor = e.defaultColor;
                e.edgeControl.outputColor = e.defaultColor;
                e.edgeControl.edgeWidth = 2;
            }
        });
    }

    #region 添加删除节点

    public T CreateNode<T>(Vector2 pos) where T : BTNode
    {
        return CreateNode(typeof(T), pos) as T;
    }

    public BTNode CreateNode(System.Type type, Vector2 pos)
    {
        var node = ScriptableObject.CreateInstance(type) as BTNode;
        node.name = type.Name;
        node.guid = GUID.Generate().ToString();
        node.position = pos;

        Undo.RecordObject(_tree, "BT (CreateNode)");
        _tree.Nodes.Add(node);

        if (!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(node, _tree);
        }

        Undo.RegisterCreatedObjectUndo(node, "BT (CreateNode)");
        AssetDatabase.SaveAssets();

        return node;
    }

    public void DeleteNode(BTNode node)
    {
        Undo.RecordObject(_tree, "BT (DeleteNode)");

        _tree.Nodes.Remove(node);
        //AssetDatabase.RemoveObjectFromAsset(node);
        Undo.DestroyObjectImmediate(node); //可撤销的销毁

        AssetDatabase.SaveAssets();
    }

    public void AddChild(BTNode parent, BTNode child)
    {
        Undo.RecordObject(parent, "BT (AddChild)");

        switch (parent)
        {
            case DecoratorNode decoratorNode:
                decoratorNode.Child = child;
                break;
            case CompositeNode compositeNode:
                compositeNode.AddChild(child);
                break;
            case StartNode startNode:
                startNode.Child = child;
                break;
            default:
                break;
        }

        EditorUtility.SetDirty(_tree);
    }

    public void RemoveChild(BTNode parent, BTNode child)
    {
        Undo.RecordObject(parent, "BT (RemoveChild)");

        switch (parent)
        {
            case DecoratorNode decoratorNode:
                decoratorNode.Child = null;
                break;
            case CompositeNode compositeNode:
                compositeNode.RemoveChild(child);
                break;
            case StartNode startNode:
                startNode.Child = null;
                break;
            default:
                break;
        }

        EditorUtility.SetDirty(_tree);
    }

    #endregion
}
