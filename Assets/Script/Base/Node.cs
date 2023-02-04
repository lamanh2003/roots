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
        public Material normalMaterial;
        public Material highlightMaterial;

        public EdgeCollider2D col;
        public List<Node> childNode;
        public float angle;
        public RopeBridge rope;
        private LineColor _lineColor;
        public int nodeHeight;

        private float orignalWidth;
        public int ChildCount => childNode.Count;

        private List<Node> AllChild
        {
            get
            {
                List<Node> rt = new List<Node>();
                Find(this);
                return rt;
                void Find(Node cur)
                {
                    foreach (var nTmp in cur.childNode)
                    {
                        rt.Add(nTmp);
                        Find(nTmp);
                    }
                }
            }
        }

        public int Deep
        {
            get
            {
                return Find(this);

                int Find(Node cur)
                {
                    int rt = 0;
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
        
        public void DestroyNode(Node parentNode)
        {
            parentNode.childNode.Remove(this);
            DestroyAnim();
        }

        public void UpdateParentNode(Node parentNode, Node oldParentNode)
        {
            transform.SetParent(parentNode.transform);
            oldParentNode.childNode.Remove(this);

        }

        private void MoveTo(Vector2 locateDiff, float angleDiff)
        {
            
        }

        private void DestroyAnim()
        {
            var allChild = AllChild;
            Debug.Log(allChild.Count);
            Fade(0.6f,()=>Destroy(gameObject));
            foreach (var nTmp in allChild)
            {
                nTmp.Fade(0.6f);
            }

        }

        private void Fade(float time,Action onComplete = null)
        {
            GetComponent<SpriteRenderer>().DOFade(0f, time).OnComplete(()=>onComplete?.Invoke());
            rope.Fade(time);
        }
        

        public enum LineColor
        {
            Any = 0,
            Pink = 1,
            Green = 2,
            Blue = 3
        }
        public void SetCollider(Vector2 endPoint)
        {
            List<Vector2> points = new List<Vector2>();
            points.Add(Vector2.zero);
            points.Add(endPoint);
            col.SetPoints(points);
        }
        public void Highlight()
        {
            orignalWidth = rope.lineRenderer.startWidth;
            rope.lineRenderer.material = highlightMaterial;

            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0.0f, 1.0f);
            curve.AddKey(0.5f, 1.8f);
            curve.AddKey(1.0f, 1.0f);

            rope.lineRenderer.widthCurve = curve;
            rope.lineRenderer.widthMultiplier = orignalWidth;
        }
        public void UnHighlight()
        {
            rope.lineRenderer.material = normalMaterial;

            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0.0f, 1.0f);
            curve.AddKey(0.5f, 1.0f);
            curve.AddKey(1.0f, 1.0f);

            rope.lineRenderer.widthCurve = curve;
            rope.lineRenderer.widthMultiplier = orignalWidth;
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