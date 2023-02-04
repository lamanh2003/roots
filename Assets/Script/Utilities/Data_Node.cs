using System;
using System.Collections.Generic;
using Base;

[Serializable]
public class Data_Node
{
    public Data_Node_Type dataNodeType;
    public Node.LineColor lineColor;
    public List<Data_Node> childNodes;
}

public enum Data_Node_Type
{
    NormalColor
}
