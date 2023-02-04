using System.Collections;
using System.Collections.Generic;
using Base;
using Sirenix.OdinInspector;
using UnityEngine;

public class Data_EditorUtility : MonoBehaviour
{
    public Transform rootNodeTransform;
    
    public int level;
    
    [Button]
    public void CreateLevelData()
    {
        List<Data_Node> firstLevelNodes = new List<Data_Node>();
        foreach (Transform childOfRoot in rootNodeTransform)
        {
            Data_Node dataNode_1 = new Data_Node();
            dataNode_1.dataNodeType = Data_Node_Type.NormalColor;
            dataNode_1.lineColor = childOfRoot.GetComponent<Node>().lineColor;
            dataNode_1.childNodes = new List<Data_Node>();
            Transform transformToCheck = childOfRoot;
            while (true)
            {
                if (transformToCheck.childCount == 0)
                {
                    break;
                }

                foreach (Transform childOfTransformToCheck in transformToCheck)
                {
                }
            }
        }
    }
}
