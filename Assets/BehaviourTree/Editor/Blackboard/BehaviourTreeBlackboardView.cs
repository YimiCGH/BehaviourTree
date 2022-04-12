using System;
using System.Collections.Generic;
using System.Linq;
using BT;
using BT.Blackboard;
using BT.Util;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

[System.Serializable]
public class BehaviourTreeBlackboardView :Blackboard
{
    //添加属性菜单
    GenericMenu m_AddBlackboardItemMenu;
    private BlackboardController m_blackBoardCtrl;

    public List<BlackboardProperty> Properties;
    public List<VisualElement> PropertyContainers;

    public BehaviourTreeBlackboardView(VisualElement root,GraphView graphView)
    {
        Properties = new List<BlackboardProperty>();
        PropertyContainers = new List<VisualElement>();
        this.SetPosition(new Rect(10, 50, 240, 420));
        /*
        var addBtn = this.Q<Button>("addButton");
        addBtn.clickable.clicked += OnClickAddBlackboardItem;
        */
        m_blackBoardCtrl = new BlackboardController(this);
        Add(new BlackboardSection{title = "属性"});


        addItemRequested = OnClickAddBlackboardItem;
        editTextRequested = EditText;
        graphView.Add(this);
    }

    private void EditText(Blackboard blackboard, VisualElement element, string newName)
    {
        var field = (BlackboardField) element;
        var oldName = field.text;
        if (Properties.Any(p => p.PropertyName == newName))
        {
            EditorUtility.DisplayDialog("Error", "黑板中已有同名属性，请重命名", "确定");
            return;
        }

        field.text = newName;
        // 更新属性名
        var changeIndex = Properties.FindIndex(p => p.PropertyName == oldName);
        Properties[changeIndex].PropertyName = newName;
    }


    void OnClickAddBlackboardItem(Blackboard _blackboard)
    {
        InitializeAddBlackboardItemMenu();
        ShowAddPropertyMenu();
    }

    public void AddField(BlackboardProperty _property)
    {
        
        var propertyDefaultName = _property.PropertyName;
        while (Properties.Any(p => p.PropertyName == propertyDefaultName))
        {
            propertyDefaultName = $"{propertyDefaultName}(1)"; // XXX || XXX(1) || XXX(1)(1)
        }

        _property.PropertyName = propertyDefaultName;
        Properties.Add(_property);
        
        var container = new VisualElement();
        PropertyContainers.Add(container);
        
        container.name = "container";
        BlackboardField field = new BlackboardField { text = _property.PropertyName, typeText = _property.PropertyType};
        container.Add(field);

        TextField textPropertyValue = new TextField("Value")
        {
            value = _property.PropertyValue
        };
        
        textPropertyValue.RegisterValueChangedCallback(e =>
        {
            var changeIndex = Properties.FindIndex(p => p.PropertyName == _property.PropertyName);
            Properties[changeIndex].PropertyValue = e.newValue;
        });

        var blackboardValueRow = new BlackboardRow(field,textPropertyValue);
        container.Add(blackboardValueRow);
        
        
        Add(container);
        container.RegisterCallback<ContextualMenuPopulateEvent>(OnRightClickProperty);
        
        field.OpenTextEditor();//新建
    }

    void InitializeAddBlackboardItemMenu() { 
        m_AddBlackboardItemMenu = new GenericMenu();
        m_blackBoardCtrl.InitPropertyType(m_AddBlackboardItemMenu);
    }

    void ShowAddPropertyMenu()
    {
        m_AddBlackboardItemMenu.ShowAsContext();
    }

    void OnRightClickProperty(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction("Delete",RemoveProperty);
    }
 
    private void RemoveProperty(DropdownMenuAction obj)
    {
        foreach (var select in graphView.selection)
        {
            var propertyField = select as BlackboardField;
            if (propertyField != null)
            {
                var changeIndex = Properties.FindIndex(p => p.PropertyName == propertyField.text);
                var property = Properties[changeIndex];
                var container = PropertyContainers[changeIndex];
                Properties.RemoveAt(changeIndex);
                PropertyContainers.RemoveAt(changeIndex);
                Remove(container);
                return;
            }
        }
    }


    public void ClearBlackboard()
    {
        for (int i = Properties.Count - 1; i >= 0; i--)
        {
            var container = PropertyContainers[i];
            Remove(container);
        }
        
        Properties.Clear();
        PropertyContainers.Clear();
    }

    public void LoadBlackboard()
    {
        var path = EditorUtility.OpenFilePanel("打开行为树", "Assets/BehaviourTree/BB", "asset");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        path = PathUtil.ToPrjectPath(path);
        var asset = AssetDatabase.LoadAssetAtPath<BTBlackboard>(path);
        if (asset == null) {
            Debug.LogError("打开失败,"+path);
            return;
        }

        ClearBlackboard();
        foreach (var property in asset.Properties)
        {
            AddField(property);
        }
    }

    public void SaveBlackboard()
    {
        var path =  EditorUtility.SaveFilePanel("保存黑板","Assets/BehaviourTree/BB","BB_New","asset");
        if (string.IsNullOrEmpty(path)) {
            return;
        }
        path = PathUtil.ToPrjectPath(path);

        var bt = ScriptableObject.CreateInstance<BTBlackboard>();
        bt.Properties = new List<BlackboardProperty>(Properties);
        AssetDatabase.CreateAsset(bt, path);
        Selection.activeObject = bt;
        AssetDatabase.Refresh();
    }

    public void ExportToLua()
    {
        
    }
}
