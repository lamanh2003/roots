using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Controller
{
    public class NodeController : MonoBehaviour
    {
        #region AssetDefine

        [SerializeField] private Node nodePrefab;
        public const float LineLength = 1f;

        #endregion

        private Node _rootNode;

        private List<Node> match3CheckSave;

        public void Init()
        {
            match3CheckSave = new List<Node>();
        }

        private void Start()
        {
            _rootNode = GameObject.FindGameObjectWithTag("rootNode").GetComponent<Node>();
            AddNode(_rootNode);
            AddNode(_rootNode);
            AddNode(_rootNode);
            AddNode(_rootNode);
            AddNode(_rootNode);
            AddNode(_rootNode);
            AddNode(_rootNode);
        }

        public void AddNode(Node parentNode)
        {
            var angle = NextAngle(parentNode);
            var color = (Node.LineColor)Random.Range(1, 4);
            AddNode(parentNode, angle, color);
        }

        private float NextAngle(Node parentNode)
        {
            if (parentNode.CompareTag("rootNode"))
            {
                var allAngle = parentNode.childNode.ConvertAll(nTmp => nTmp.angle).ToArray();
                float rt = 0f;
                while (true)
                {
                    if (!allAngle.Any(aTmp => Math.Abs(aTmp - rt) < 1f))
                    {
                        return rt;
                    }

                    rt += 45;
                }
            }

            return parentNode.ChildCount switch
            {
                0 => parentNode.angle + 22.5f,
                _ => parentNode.angle - 22.5f
            };
        }

        private void AddNode(Node parentNode, float angle, Node.LineColor lineColor)
        {
            var parentPosition = parentNode.transform.position;
            var desLocate = (Vector2)parentPosition + MathUtilities.DegreeToVector2(angle) * LineLength;
            Node instance = Instantiate(nodePrefab, desLocate, Quaternion.identity, parentNode.transform);
            parentNode.childNode.Add(instance);
            instance.angle = angle;
            instance.lineColor = lineColor;
            instance.rope.Init(parentPosition, desLocate, lineColor);
        }

        public void ChangeNodeColor(Node target)
        {
            ChangeNodeColor(target, target.lineColor.NextLineColor());
        }

        private void ChangeNodeColor(Node target, Node.LineColor lineColor)
        {
            target.lineColor = lineColor;
        }

        public void CheckAll()
        {
            match3CheckSave.Clear();
            Action onCheckAll = default;
            CheckNext(_rootNode);
            onCheckAll?.Invoke();

            void CheckNext(Node currentNode)
            {
                foreach (var nTmp in currentNode.childNode)
                {
                    var lTmp = CheckMatch3(nTmp);
                    if (lTmp.Count >= 3)
                    {
                        match3CheckSave.AddRange(lTmp);
                        onCheckAll += () => ClaimNodeList(lTmp);
                    }
                    else
                    {
                        CheckNext(nTmp);
                    }
                }
            }
        }

        public List<Node> CheckMatch3(Node checkNode)
        {
            List<Node> res = new List<Node>();
            if (match3CheckSave.Contains(checkNode))
            {
                return res;
            }

            foreach (var nTmp in checkNode.childNode.Except(match3CheckSave))
            {
                if (nTmp.lineColor.ColorEquals(checkNode.lineColor))
                {
                    var tmpM3 = CheckMatch3(nTmp);
                    if (tmpM3.Count + 1 > res.Count)
                    {
                        res.Clear();
                        res.Add(checkNode);
                        res.AddRange(tmpM3);
                    }
                }
            }

            return res;
        }

        private void ClaimNodeList(List<Node> nodeList)
        {
            for (int i = 0; i < nodeList.Count; i++)
            {
                nodeList[i].ClaimNode();
                if (i > 0)
                {
                    //nodeList[i].DestroyNode(nodeList[0]);
                }
            }
        }
    }
}