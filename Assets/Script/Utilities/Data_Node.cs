using System;
using System.Collections.Generic;
using Base;
using Sirenix.OdinInspector;
using UnityEngine;

[Searchable]
public class Data_Node
{
    public Data_Node_Type dataNodeType;
    public Node.LineColor lineColor;
    public List<Data_Node> childNodes;

    public Data_Node()
    {
        childNodes = new List<Data_Node>();
    }

    public Data_Node(Node.LineColor lineColor)
    {
        dataNodeType = Data_Node_Type.NormalColor;
        this.lineColor = lineColor;
        childNodes = new List<Data_Node>();
    }
}

public enum Data_Node_Type
{
    NormalColor
}
