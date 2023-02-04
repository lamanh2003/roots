using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using UnityEngine;
using Utilities;

namespace Base
{
    public class Node : MonoBehaviour
    {
        public List<Node> nextNode;
        public RopeBridge rope;
        private LineColor _lineColor;

        public LineColor lineColor
        {
            get => _lineColor;
            set
            {
                _lineColor = value;
                rope.UpdateLineColor(value);
            }
        }

        private void Awake()
        {
            nextNode = new List<Node>();
        }

        public enum LineColor
        {
            Any = 0,
            Pink = 1,
            Green = 2,
            Blue = 3
        }
    }

    public static class LineColorExtension
    {
        public static Color ToColor(this Node.LineColor lineColor)
        {
            switch (lineColor)
            {
                case Node.LineColor.Pink:
                    return new Color(234f / 255f, 77f / 255f, 103f / 255f);
                case Node.LineColor.Green:
                    return new Color(174f / 255f, 207f / 255f, 59f / 255f);
                case Node.LineColor.Blue:
                    return new Color(55f / 255f, 147f / 255f, 184f / 255f);
                default:
                    throw new ArgumentOutOfRangeException(nameof(lineColor), lineColor, null);
            }

            return Color.black;
        }

        public static Node.LineColor NextLineColor(this Node.LineColor lineColor)
        {
            if (lineColor == Node.LineColor.Blue)
                return Node.LineColor.Pink;

            return (Node.LineColor)((int)lineColor + 1);
        }
    }
}