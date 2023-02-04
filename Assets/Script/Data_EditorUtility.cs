using Base;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class Data_EditorUtility : MonoBehaviour
{
    public Transform rootNodeTransform;
    public Data_Level lvExport;
    
    [Button]
    public void CreateLevelData()
    {
        Data_Node rootData = new Data_Node();
        Create(rootNodeTransform,rootData);

        void Create(Transform now,Data_Node dNow)
        {
            for (int i = 0; i < now.childCount; i++)
            {
                var tTmp = now.GetChild(i);
                var dTmp = new Data_Node
                {
                    lineColor = tTmp.GetComponent<Node>().lineColor
                };
                dNow.childNodes.Add(dTmp);
                Create(tTmp,dTmp);
            }
        }
    }
}
