using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using BT;
using System.Collections.Generic;
using System.Linq;
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
            _tree.rootNode = _tree.CreateNode<StartNode>();
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
            port.node != startPort.node
        ).ToList();
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null) {
            graphViewChange.elementsToRemove.ForEach( element => {
                NodeView nodeView = element as NodeView;
                if (nodeView != null) {
                    _tree.DeleteNode(nodeView.Node);
                }

                Edge edge = element as Edge;
                if (edge != null) {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    _tree.RemoveChild(parentView.Node, childView.Node);
                }
            });
        }

        if (graphViewChange.edgesToCreate != null) {
            graphViewChange.edgesToCreate.ForEach( edge => {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;
                _tree.AddChild(parentView.Node,childView.Node);
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

    /// <summary>
    /// 右键菜单命令
    /// </summary>
    /// <param name="evt"></param>

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        //base.BuildContextualMenu(evt);
        {
            var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}]/{type.Name}",(a) => CreateNode(type));
            }
        }
        {
            var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}]/{type.Name}", (a) => CreateNode(type));
            }
        }
        {
            var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}]/{type.Name}", (a) => CreateNode(type));
            }
        }
    }

    void CreateNode(System.Type type) {
        var node = _tree.CreateNode(type);
        CreateNodeView(node);
    }

    void CreateNodeView(BTNode node) {
        string uiFile ;//各种节点可以自定义界面
        switch (node)
        {            
            case SequencerNode sequencerNode:
                uiFile = "Assets/BehaviourTree/Editor/CompositeNodeView.uxml";
                break;
            case ParallelNode parallelNode:
                uiFile = "Assets/BehaviourTree/Editor/CompositeNodeView.uxml";
                break;
            case SelectorNode selectorNode:
                uiFile = "Assets/BehaviourTree/Editor/SelectorNodeView.uxml";
                break;
            default:
                uiFile = "Assets/BehaviourTree/Editor/NodeView.uxml";
                break;
        }

        NodeView nodeView = new NodeView(node, uiFile);
        nodeView.OnNodeSelected = OnNodeSelected;
        AddElement(nodeView);
    }
    void CreateEdge(NodeView parent, NodeView child) {
        var edge = parent.Output.ConnectTo(child.Input);
        AddElement(edge);
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
}
