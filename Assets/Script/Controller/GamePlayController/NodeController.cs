﻿using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using UnityEngine;
using UnityEngine.Serialization;
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
        public List<Node> listDrawNodes;
        public int _id;

        [FormerlySerializedAs("_rootNode")] public Node rootNode;

        private List<Node> match3CheckSave;

        public void Init()
        {
            match3CheckSave = new List<Node>();
        }
        

        public Node AddNode(Node parentNode, bool isLeftPriority = true, Node.LineColor? lineColor = null)
        {
            var angle = NextAngle(parentNode, isLeftPriority);
            GamePlayController.Singleton.levelController.config.colorCycle.Shuffle();
            var color = lineColor ?? GamePlayController.Singleton.levelController.config.colorCycle[0];
            return AddNode(parentNode, angle, color);
        }

        private float NextAngle(Node parentNode, bool isLeftPriority = true)
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

            float farSize = Mathf.Clamp(parentNode.nodeHeight * 0.5f, 0,10f) * parentNode.nodeHeight%2==0?1:-1;
            return parentNode.ChildCount switch
            {
                0 => isLeftPriority ? parentNode.angle + 22.5f - farSize : parentNode.angle - 22.5f + farSize ,
                _ => isLeftPriority ? parentNode.angle - 22.5f + farSize : parentNode.angle + 22.5f - farSize 
            } ;
        }

        private Node AddNode(Node parentNode, float angle, Node.LineColor lineColor)
        {
            var parentPosition = parentNode.transform.position;
            var desLocate = (Vector2)parentPosition + MathUtilities.DegreeToVector2(angle) * LineLength;
            Node instance = Instantiate(nodePrefab, desLocate, Quaternion.identity, parentNode.transform);
            parentNode.childNode.Add(instance);
            instance.angle = angle;
            instance.lineColor = lineColor;

            instance.rope.Init(parentNode.transform, instance.transform, lineColor);


            var iTransform = instance.transform;
            Vector2 endPoint = (parentNode.transform.position - iTransform.position);
            Vector2 realEndPoint = new Vector2(endPoint.x / iTransform.localScale.x, endPoint.y / iTransform.localScale.y);
            instance.parent = parentNode;
            instance.SetCollider(realEndPoint);
            instance.name = _id.ToString();
            _id++;
            UpdateNodeHeight();
            return instance;
        }

        public void ChangeNodeColor(Node target)
        {
            ChangeNodeColor(target, target.lineColor.NextLineColor());
        }

        private void ChangeNodeColor(Node target, Node.LineColor lineColor)
        {
            target.lineColor = lineColor;
        }

        public bool CheckTurn()
        {
            if (listDrawNodes.Count == 0)
            {
                return false;
            }
            bool[] needCheck = listDrawNodes.ConvertAll(_=>false).ToArray();
            foreach (var nTmp in listDrawNodes) nTmp._lineColor = nTmp.lineColor.NextLineColor();
            match3CheckSave.Clear();
            var lTmpRoot = CheckMatch3(rootNode);
            if (lTmpRoot.Count >= 3)
            {
                match3CheckSave.AddRange(lTmpRoot);
            }

            CheckNext(rootNode);

            for (int i = 0; i < listDrawNodes.Count; i++)
            {
                if (match3CheckSave.Contains(listDrawNodes[i]))
                {
                    needCheck[i] = true;
                }
            }
            foreach (var nTmp in listDrawNodes) nTmp._lineColor = nTmp.lineColor.PreviousLineColor();

            return needCheck.All(bTmp => bTmp);

            void CheckNext(Node currentNode)
            {
                foreach (var nTmp in currentNode.childNode)
                {
                    var lTmp = CheckMatch3(nTmp);
                    if (lTmp.Count >= 3)
                    {
                        match3CheckSave.AddRange(lTmp);
                    }
                    else
                    {
                        CheckNext(nTmp);
                    }
                }
            }
        }

        public void ClaimAll()
        {
            match3CheckSave.Clear();
            Action onCheckAll = default;
            var lTmpRoot = CheckMatch3(rootNode);
            if (lTmpRoot.Count >= 4)
            {
                GamePlayController.Singleton.point += lTmpRoot.Count;
                GamePlayController.Singleton.pointTMP.SetText( GamePlayController.Singleton.point.ToString());
                match3CheckSave.AddRange(lTmpRoot);
                onCheckAll += () => ClaimNodeList(lTmpRoot);
            }

            CheckNext(rootNode);
            onCheckAll?.Invoke();

            void CheckNext(Node currentNode)
            {
                foreach (var nTmp in currentNode.childNode)
                {
                    var lTmp = CheckMatch3(nTmp);
                    if (lTmp.Count >= 4)
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

        private List<Node> CheckMatch3(Node checkNode)
        {
            List<Node> res = new List<Node>();
            if (match3CheckSave.Contains(checkNode))
            {
                return res;
            }
            res.Add(checkNode);
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
            Node longestNode = null;
            Node parentLongestNode = null;
            int longestNodeDeep = 0;
            for (int i = 0; i < nodeList.Count; i++)
            {
                nodeList[i].ClaimNode();
                if (i > 0)
                {
                    foreach (var nTmp in nodeList[i].childNode)
                    {
                        if (match3CheckSave.Contains(nTmp))
                            continue;
                        var dTmp = nTmp.Deep;
                        if (dTmp > longestNodeDeep)
                        {
                            longestNodeDeep = dTmp;
                            parentLongestNode = nodeList[i];
                            longestNode = nTmp;
                        }
                    }
                }
            }

            //if (longestNode != null) longestNode.UpdateParentNode(nodeList[0],parentLongestNode);
            //longestNode.transform.SetParent();
            if (nodeList[0] == rootNode)
                nodeList[1].DestroyNode(nodeList[0]);
            else
                nodeList[0].DestroyNode(nodeList[0].parent);
            void ReSpawn(Node current)
            {
                
            }
        }
        public void UnHighlightAll()
        {
            for(int i = 0; i < listDrawNodes.Count; i++)
            {
                listDrawNodes[i].UnHighlight();
            }
        }

        public void UpdateNodeHeight()
        {
            Recur(rootNode,0);
            void Recur(Node cur, int deep)
            {
                deep++;
                foreach (var nTmp in cur.childNode)
                {
                    nTmp.nodeHeight = deep;
                    Recur(nTmp,deep);
                }
            }
        }
    }
}