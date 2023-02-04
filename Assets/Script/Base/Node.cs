using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using DG.Tweening;
using UnityEngine;
using Utilities;

namespace Base
{
    public class Node : MonoBehaviour
    {
        public List<Node> childNode;
        public float angle;
        public RopeBridge rope;
        private LineColor _lineColor;
        public int ChildCount => childNode.Count;

        private int deep
        {
            get
            {
                int rt = 0;
                return Find(this);

                int Find(Node cur)
                {
                    foreach (var nTmp in cur.childNode)
                    {
                        rt = Mathf.Max(rt, Find(nTmp));
                    }

                    return rt + 1;
                }
            }
        }

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
            childNode = new List<Node>();
        }

        public void ClaimNode()
        {
            
        }
        
        public void DestroyNode(Node parentNode,List<Node> exceptChildNode)
        {
            parentNode.childNode.Remove(this);
            childNode = childNode.Except(exceptChildNode).ToList();
            parentNode.childNode.AddRange(childNode);
            if (ChildCount == 0)
            {
                return;
            }
            Node longestNode = null;
            int longestNodeDeep = 0;
            foreach (var nTmp in childNode)
            {
                var dTmp = nTmp.deep;
                if (longestNodeDeep < dTmp)
                {
                    longestNodeDeep = dTmp;
                    longestNode = nTmp;
                }
            }
            //Xóa các cành ngắn
            foreach (var nTmp in childNode.Where(nTmp => nTmp != longestNode))
            {
                nTmp.DestroyNodeForce();
            }
            UpdateParentNode(longestNode);
        }

        public void DestroyNodeForce()
        {
            
        }

        public void UpdateParentNode(Node parentNode)
        {
            
        }

        private void MoveTo(Vector2 locateDiff, float angleDiff)
        {
            
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

        public static bool ColorEquals(this Node.LineColor color1, Node.LineColor color2)
        {
            if (color1 == Node.LineColor.Any || color2 == Node.LineColor.Any)
            {
                return true;
            }

            return color1 == color2;
        }
    }
}