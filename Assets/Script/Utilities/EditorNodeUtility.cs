using System;
using Base;
using Controller;
using Sirenix.OdinInspector;
using UnityEngine;

public class EditorNodeUtility : MonoBehaviour
{
    public Node.LineColor chosenColor;
    public Node node;

    [Button]
    public void AddLeftNode()
    {
        GamePlayController.Singleton.nodeController.AddNode(this.node, isLeftPriority: true, chosenColor);
    }
    
    [Button]
    public void AddRightNode()
    {
        GamePlayController.Singleton.nodeController.AddNode(this.node, isLeftPriority: true, chosenColor);
    }
}
