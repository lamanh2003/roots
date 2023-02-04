using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Utilities;

namespace Base
{
    public class Node : MonoBehaviour
    {
        public Material normalMaterial;
        public Material highlightMaterial;

        public bool isSpawnNextTurn;

        public EdgeCollider2D col;
        public Node parent;
        public List<Node> childNode;
        public float angle;
        public RopeBridge rope;
        public LineColor _lineColor;
        public int nodeHeight;

        [HideInInspector] public bool isConnected;

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
        public Vector2 GetCenterPoint()
        {
            Vector2 result = Vector2.zero;
            if(rope.lineRenderer.positionCount > 1)
            {
                result = rope.lineRenderer.GetPosition((int)(rope.lineRenderer.positionCount / 2));
            }
            return result;
        }

        public List<Node> AllFarestNode
        {
            get
            {
                return AllChild.Where(nTmp => nTmp.ChildCount == 0).ToList();
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
            Fade(0.6f, () => Destroy(gameObject));
            foreach (var nTmp in allChild)
            {
                nTmp.Fade(0.6f);
            }
        }

        private void Fade(float time, Action onComplete = null)
        {
            GetComponent<SpriteRenderer>().DOFade(0f, time).OnComplete(() => onComplete?.Invoke());
            rope.Fade(time);
        }

        [Serializable]
        public enum LineColor
        {
            Any = 0,
            Pink = 1,
            Green = 2,
            Blue = 3,
            Purple = 4,
            Orange = 5,
            Random = 6,
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
            isConnected = true;

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
            isConnected = false;
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
                case Node.LineColor.Purple:
                    return new Color(156f / 255f, 102f / 255f, 196f / 255f);
                case Node.LineColor.Orange:
                    return new Color(247f / 255f, 147f / 255f, 30f / 255f);
                default:
                    throw new ArgumentOutOfRangeException(nameof(lineColor), lineColor, null);
            }

            return Color.black;
        }

        public static Node.LineColor NextLineColor(this Node.LineColor lineColor)
        {
            if (lineColor == Node.LineColor.Orange)
                return Node.LineColor.Pink;

            return (Node.LineColor)((int)lineColor + 1);
        }

        public static Node.LineColor PreviousLineColor(this Node.LineColor lineColor)
        {
            if (lineColor == Node.LineColor.Pink)
                return Node.LineColor.Orange;

            return (Node.LineColor)((int)lineColor - 1);
        }

        public static bool ColorEquals(this Node.LineColor color1, Node.LineColor color2)
        {
            if (color1 == Node.LineColor.Any || color2 == Node.LineColor.Any)
            {
                return true;
            }

            return color1 == color2;
        }

        public static Node.LineColor GetLineColor(this Color color)
        {
            if (color == new Color(234f / 255f, 77f / 255f, 103f / 255f))
                return Node.LineColor.Pink;
            if (color == new Color(174f / 255f, 207f / 255f, 59f / 255f)) return Node.LineColor.Green;
            if (color == new Color(55f / 255f, 147f / 255f, 184f / 255f))
                return Node.LineColor.Blue;

            if (color == new Color(156f / 255f, 102f / 255f, 196f / 255f))
                return Node.LineColor.Purple;

            return Node.LineColor.Orange;
        }
    }
}