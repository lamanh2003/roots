#if UNITY_EDITOR
using Base;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public static class GlobalNodeUtility
{
    [MenuItem("Utility/Add Left Node of Selected Node #L")]
    public static void CreateLeftNode()
    {
        var nodeObj = Selection.activeObject;
        var nodeEditorNodeUtility = nodeObj.GetComponent<EditorNodeUtility>();
        if (nodeEditorNodeUtility != null)
        {
            nodeEditorNodeUtility.chosenColor = (Node.LineColor)Random.Range(1, 4);
            nodeEditorNodeUtility.AddLeftNode();
        }
    }
    [MenuItem("Utility/Add Right Node of Selected Node #O")]
    public static void CreateRightNode()
    {
        var nodeObj = Selection.activeObject;
        var nodeEditorNodeUtility = nodeObj.GetComponent<EditorNodeUtility>();
        if (nodeEditorNodeUtility != null)
        {
            nodeEditorNodeUtility.chosenColor = (Node.LineColor)Random.Range(1, 4);
            nodeEditorNodeUtility.AddRightNode();
        }
    }
}
#endif