using System;
using System.Collections.Generic;
using Base;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "CreateLevel")]
public class Data_Level: SerializedScriptableObject
{
    public int level;
    public Data_Node rootData;
    public List<Node.LineColor> colorCycle;
}
