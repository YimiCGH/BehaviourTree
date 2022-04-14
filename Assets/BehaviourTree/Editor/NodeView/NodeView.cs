using BT;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
public class NodeView : Node
{
    public Action<NodeView> OnNodeSelected;
    public BTNode Node;

    SerializedObject Node_SerializedObj;

    private Port inputPort;
    private List<Port> outPutPorts;

    public NodeView(BTNode node)
    {
        Node = node;
        title = node.name;
        LoadStyleSheet();
        
        viewDataKey = node.guid;
        style.left = node.position.x;
        style.top = node.position.y;
        if (Node is CompositeNode)
            style.minWidth = 250;

        outPutPorts = new List<Port>();
        InitTitleContainer();
        InitInputPorts();
        InitOutputPorts();
        SetupClasses();
        InitMainContain();

        //BindInspector();

        node.OnEndEvent = OnNodeEnd;
        RefreshExpandedState();
    }

    protected virtual void LoadStyleSheet()
    {
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/BehaviourTree/Editor/NodeViewStyle.uss");   
        styleSheets.Add(styleSheet);
    }

    protected virtual void InitTitleContainer()
    {
        var btn = new Button(() => { Debug.Log("Open Node Script");});
        btn.text = "Open";
        titleContainer.Add(btn);        
    }

    void InitMainContain()
    {
        Node_SerializedObj = new SerializedObject(Node);
        var descriptionLabel = new Label("");
        descriptionLabel.Bind(Node_SerializedObj);//绑定序列化对象
        descriptionLabel.bindingPath = "Description";//绑定序列化对象的属性名称
        mainContainer.Add(descriptionLabel);
    }
    protected virtual void SetupClasses()
    {
        switch (Node)
        {
            case ActionNode actionNode:
                AddToClassList("action");
                break;            
            case CompositeNode compositeNode:
                AddToClassList("composite");
                break;
            case DecoratorNode decoratorNode:
                AddToClassList("decorator");
                break;
            case StartNode startNode:
                AddToClassList("start");
                break;
            case ValueNode valueNode:
                AddToClassList("value");
                break;
            default:
                break;
        }
    }

    protected virtual void InitInputPorts()
    {
        CreateInputPort("", Port.Capacity.Single, typeof(bool));
    }
    protected virtual void InitOutputPorts()
    {       
        CreateOutputPort("", Port.Capacity.Single, typeof(bool));
    }

    protected Port CreateOutputPort(string portName,Port.Capacity capacity,Type type)
    {
        var port = InstantiatePort(Orientation.Horizontal, Direction.Output, capacity, type);
        port.portName = portName;
        outputContainer.Add(port);
        outPutPorts.Add(port);
        RefreshPorts();
        RefreshExpandedState();   
        return port;
    }
    protected Port CreateInputPort(string portName,Port.Capacity capacity,Type type)
    {
        if (inputPort != null)
        {
            Debug.LogWarning("已有输入接口，不允许有多个输入");
            return inputPort;
        }

        var port = InstantiatePort(Orientation.Horizontal, Direction.Input, capacity, type);
        port.portName = portName;
        inputContainer.Add(port);
        RefreshPorts();
        RefreshExpandedState();
        inputPort = port;
        return port;
    }

    public Edge ConnectNodeView(NodeView targetNodeView)
    {
        Port outputPort = null;
        foreach (var port in outPutPorts)
        {
            if (!port.connected)
            {
                outputPort = port;
                break;
            }
        }

        if (outputPort == null)
        {
            outputPort = CreateOutputPort("", Port.Capacity.Single, typeof(bool));
        }
        
        var port2 = targetNodeView.inputPort;
        if (port2 == null)
        {
            Debug.LogError($"{targetNodeView.title} 缺少输入接口");
            return null;
        }

        return outputPort.ConnectTo(port2);
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(Node,"BT (Set Position)");
        Node.position.x = newPos.xMin;
        Node.position.y = newPos.yMin;
        EditorUtility.SetDirty(Node);
    }

    public override void OnSelected()
    {
        base.OnSelected();
        if (OnNodeSelected != null)
            OnNodeSelected.Invoke(this);
    }
    public void SortChildren() {
        var composite = Node as CompositeNode;
        if (composite == null) {
            return;
        }

        composite.Children.Sort(SortByVerticalPosition);
    }

    private int SortByHorizontalPosition(BTNode left, BTNode right)
    {
        return left.position.x > right.position.x ? 1 : -1;
    }
    private int SortByVerticalPosition(BTNode top, BTNode dowm)
    {
        return top.position.y > dowm.position.y ? 1 : -1;
    }
    
    VisualElement statebar;
    public void UpdateState() {
        if (!Application.isPlaying) {
            return;
        }

        if (statebar == null)
        {
            statebar = this.Q<VisualElement>("statebar");
        }


        RemoveFromClassList("running");
        RemoveFromClassList("failure");
        RemoveFromClassList("success");


        switch (Node.State)
        {
            case E_State.Running:
                if (Node.Started) { //节点开始后才加上running 标签
                    AddToClassList("running");
                }                
                break;
            case E_State.Failure:
                AddToClassList("failure");
                break;
            case E_State.Success:
                AddToClassList("success");
                break;
            default:
                break;
        }
    }
    private void OnNodeEnd(BTNode obj)
    {
        this.Q<VisualElement>("statebar").style.width = new StyleLength(Length.Percent(100));     
    }
}