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

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/BehaviourTree/Editor/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);

        Undo.undoRedoPerformed += OnUndoRedo;
    }
    private void OnUndoRedo()
    {
        UpdateTreeView(_tree);
        AssetDatabase.SaveAssets();
    }

    NodeView FindNodeView(BTNode node) {
        return GetNodeByGuid(node.guid) as NodeView;
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
        nodeView.Init(node);
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
                compositeNode.Children.Add(child);
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
                compositeNode.Children.Remove(child);
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
