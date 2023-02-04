using System.IO;
using Base;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

public class Data_EditorUtility : MonoBehaviour
{
    public Transform rootNodeTransform;
    public Data_Node rootData;
    public int level;
    
    [Button]
    public void CreateLevelData()
    {
        rootData = new Data_Node();
        Create(rootNodeTransform,rootData);
        var path = Path.Combine(Application.dataPath, "level", "level_1.json");
        var json = JsonConvert.SerializeObject(rootData);
        File.WriteAllText(path,json);
        void Create(Transform now,Data_Node dNow)
        {
            for (int i = 0; i < now.childCount; i++)
            {
                var tTmp = now.GetChild(i);
                var dTmp = new Data_Node(tTmp.GetComponent<Node>().lineColor);
                dNow.childNodes.Add(dTmp);
                Create(tTmp,dTmp);
            }
        }
    }
}
