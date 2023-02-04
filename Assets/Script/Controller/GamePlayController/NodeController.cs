using System;
using Base;
using UnityEngine;
using Utilities;

namespace Controller
{
    public class NodeController: MonoBehaviour
    {
        #region AssetDefine

        [SerializeField] private Node nodePrefab;
        public const float LineLength = 1f;


        #endregion
        
        private Node _rootNode;

        public void Init()
        {
            
        }

        private void Start()
        {
            _rootNode = GameObject.FindGameObjectWithTag("rootNode").GetComponent<Node>();
            AddNode(_rootNode,0,Node.LineColor.Pink);
            AddNode(_rootNode,90,Node.LineColor.Green);
            AddNode(_rootNode,180,Node.LineColor.Blue);
            AddNode(_rootNode,270,Node.LineColor.Green);
        }


        public void AddNode(Node parentNode, float angle, Node.LineColor lineColor)
        {
            var parentPosition = parentNode.transform.position;
            var desLocate = (Vector2)parentPosition + MathUtilities.DegreeToVector2(angle) * LineLength;
            Node instance = Instantiate(nodePrefab, desLocate, Quaternion.identity, parentNode.transform);
            parentNode.nextNode.Add(instance);
            instance.lineColor = lineColor;
            instance.rope.Init(parentPosition,desLocate,lineColor);

            Vector2 endPoint = (parentNode.transform.position - instance.transform.position);
            Vector2 realEndPoint = new Vector2(endPoint.x / instance.transform.localScale.x, endPoint.y / instance.transform.localScale.y);
            Debug.LogError(endPoint.x);
            Debug.LogError(instance.transform.localScale.x);
            Debug.LogError("test " + (realEndPoint));
            instance.SetCollider(realEndPoint);
        }
       
        public void ChangeNodeColor(Node target)
        {
            ChangeNodeColor(target, target.lineColor.NextLineColor());
        }

        private void ChangeNodeColor(Node target, Node.LineColor lineColor)
        {
            target.lineColor = lineColor;
        }

        public void CheckMatch3()
        {
            
        }
    }
}