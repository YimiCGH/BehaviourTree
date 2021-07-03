using System;
using UnityEditor;
using UnityEngine.UIElements;
public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

    Editor editor;

    public InspectorView() { }

    internal void UpdateSelection(NodeView nodeView)
    {
        Clear();
        if(editor != null)
            UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(nodeView.Node);
        IMGUIContainer container = new IMGUIContainer(()=> {
            if(editor != null && editor.target)    
                editor.OnInspectorGUI(); 
        });
        Add(container);
    }
}