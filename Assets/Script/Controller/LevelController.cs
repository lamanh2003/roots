using System.Collections;
using System.Collections.Generic;
using System.IO;
using Base;
using Controller;
using Newtonsoft.Json;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public Data_Level config;
    public void Init()
    {
        Create(config.rootData,GamePlayController.Singleton.nodeController.rootNode);
        
        void Create(Data_Node dNow,Node nNow)
        {
            foreach (var dTmp in dNow.childNodes)
            {
                var nTmp = GamePlayController.Singleton.nodeController.AddNode(nNow,true,dTmp.lineColor);
                Create(dTmp,nTmp);
            }
        }
    }
}
