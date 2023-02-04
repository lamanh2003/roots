using System.IO;
using Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class Data_EditorUtility : MonoBehaviour
{
    public Transform rootNodeTransform;
    public Data_Node rootData;
    public int level;
    
    [Button]
    public void CreateLevelData()
    {
        string path = "Assets/level/0/";
        int idx = 0;
        Create(rootNodeTransform,rootData);

        void Create(Transform now,Data_Node dNow)
        {
            for (int i = 0; i < now.childCount; i++)
            {
                var tTmp = now.GetChild(i);
                var dTmp = ScriptableObject.CreateInstance<Data_Node>();
                AssetDatabase.CreateAsset(dTmp,path+idx+".asset");
                idx++;
                dTmp.lineColor = tTmp.GetComponent<Node>().rope.lineRenderer.startColor.GetLineColor();
                dNow.childNodes.Add(dTmp);
                Create(tTmp,dTmp);
            }
        }
    }
}
