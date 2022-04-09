﻿using BT;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
public class NodeView : Node
{
    public Action<NodeView> OnNodeSelected;
    public BTNode Node;
    public Port Input;
    public Port Output;

    SerializedObject Node_SerializedObj;

    public NodeView(BTNode node,string uiFile) 
        :base(uiFile)
    {
        Node = node;
        title = node.name;
        viewDataKey = node.guid;

        style.left = node.position.x;
        style.top = node.position.y;
        if (Node is CompositeNode)
            style.minWidth = 250;

        CreateInputPorts();
        CreateOutputPorts();
        SetupClasses();

        BindInspector();

        node.OnEndEvent = OnNodeEnd;
    }

    void BindInspector() {
        Node_SerializedObj = new SerializedObject(Node);
        Label descriptionLabel = this.Q<Label>("description");//通过UXML中的元素名称获取VisualElement
        descriptionLabel.Bind(Node_SerializedObj);//绑定序列化对象
        descriptionLabel.bindingPath = "Description";//绑定序列化对象的属性名称
        switch (Node)
        {
            case SequencerNode sequencerNode:
                {
                    var enumField = this.Q<EnumField>("returntype");
                    enumField.Bind(Node_SerializedObj);
                    enumField.bindingPath = "ReturnType";
                }
                break;
            case ParallelNode parallelNode:
                {
                    var enumField = this.Q<EnumField>("returntype");
                    enumField.Bind(Node_SerializedObj);
                    enumField.bindingPath = "ReturnType";
                }
                break;
            default:
                break;
        }
        if (Node is CompositeNode) {
            var prerun = this.Q<TextField>("prerun");
            if (prerun != null) {
                prerun.Bind(Node_SerializedObj);
                prerun.bindingPath = "PreRun";
            }
            
            var endrun = this.Q<TextField>("endrun");
            if (endrun != null) {
                endrun.Bind(Node_SerializedObj);
                endrun.bindingPath = "EndRun";
            }
        }
    }



    private void SetupClasses()
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
            default:
                break;
        }
    }

    void CreateInputPorts()
    {
        switch (Node)
        {
            case ActionNode actionNode:
                Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                break;
            case CompositeNode compositeNode:
                Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                break;
            case DecoratorNode decoratorNode:
                Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                break;
            case StartNode startNode:
                Input = null;
                break;
            default:
                break;
        }
        if (Input != null)
        {
            Input.portName = "";
            Input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(Input);
        }
    }
    void CreateOutputPorts()
    {
        switch (Node)
        {
            case ActionNode actionNode:                
                Output = null;
                break;
            case CompositeNode compositeNode:
                Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
                break;
            case DecoratorNode decoratorNode:
                Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
                break;
            case StartNode startNode:
                Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
                break;
            default:
                break;
        }

        if (Output != null)
        {
            Output.portName = "";
            var connector = Output.Q<VisualElement>("cap");
            Output.style.flexDirection = FlexDirection.ColumnReverse;
            
            outputContainer.Add(Output);
        }
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

        composite.Children.Sort(SortByHorizontalPosition);

    }

    private int SortByHorizontalPosition(BTNode left, BTNode right)
    {
        return left.position.x > right.position.x ? 1 : -1;
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
                    //this.Q<VisualElement>("statebar").style.width = new StyleLength(Length.Percent(50));
                    statebar.style.width = new StyleLength(Length.Percent(Node.blackboard.RunningDisplay));

                    
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