//https://www.youtube.com/@jasony4017
using System;
using System.Collections;
using System.Collections.Generic;
using Base;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utilities
{
    [RequireComponent(typeof(LineRenderer))]
    public class RopeBridge : MonoBehaviour
    {
        public Vector2 StartPoint;
        public Vector2 EndPoint;
        private Vector2 dirVec;

        [HideInInspector] public LineRenderer lineRenderer;
        private List<RopeSegment> ropeSegments;
        private List<Vector2> colPoints;
        private float ropeSegLen = 0.0025f;
        private int segmentLength = 40;
        private float lineWidth = 0.1f;
        private Vector2 forceGravity;
        private Vector2 startGravity;
        private Vector2 endGravity;
        private float gravityChangeTime;
        private float gravityTotalChangeTime;

        private int indexMousePos;

        [SerializeField] private Vector2 followTarget;

        [SerializeField] private float followTaretUpdateSpeed;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        public void UpdateLineColor(Node.LineColor lineColor)
        {
            lineRenderer.startColor = lineColor.ToColor();
            lineRenderer.endColor = lineRenderer.startColor;
        }

        private IEnumerator ChangeGravityForce()
        {
            while (true)
            {
                int rand = Random.Range(0, 4);
                startGravity = forceGravity;
                Vector2 tmpGravity = Vector2.zero;
                float gravityScale = 0.5f;
                switch (rand)
                {
                    case 0:
                        tmpGravity = new Vector2(1f, 0f);
                        break;
                    case 1:
                        tmpGravity = new Vector2(-1f, 0f);
                        break;
                    case 2:
                        tmpGravity = new Vector2(0f, 1f);
                        break;
                    case 3:
                        tmpGravity = new Vector2(0f, -1f);
                        break;
                }

                endGravity = tmpGravity * gravityScale;
                if (startGravity == endGravity)
                {
                    continue;
                }
                gravityChangeTime = 0f;
                gravityTotalChangeTime = Random.Range(1f, 3f);
                yield return new WaitForSeconds(gravityTotalChangeTime);
            }
        }
        

        public void Init(Vector2 startPoint, Vector2 endPoint, Node.LineColor lineColor)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            dirVec = (EndPoint - StartPoint).normalized;
            ropeSegments = new List<RopeSegment>();
            colPoints = new List<Vector2>();
            StartCoroutine(ChangeGravityForce());
            lineRenderer.startColor = lineColor.ToColor();
            lineRenderer.endColor = lineRenderer.startColor;
            Vector3 ropeStartPoint = StartPoint;
            

            for (int i = 0; i < segmentLength; i++)
            {
                ropeSegments.Add(new RopeSegment(ropeStartPoint));
                colPoints.Add(ropeStartPoint);
                ropeStartPoint.y -= ropeSegLen;
            }
            //col.SetPoints(colPoints);
        }

        public void UpdateLocate(Vector2 startPoint, Vector2 endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            dirVec = (EndPoint - StartPoint).normalized;
        }


        // Update is called once per frame
        void Update()
        {
            gravityChangeTime += Time.deltaTime;
            forceGravity = Vector2.Lerp(startGravity, endGravity, gravityChangeTime / gravityTotalChangeTime);
            DrawRope();
            float xStart = StartPoint.x;
            float xEnd = EndPoint.x;
            float currX = followTarget.x;

            float ratio = (currX - xStart) / (xEnd - xStart);
            if (ratio > 0)
            {
                indexMousePos = (int)(segmentLength * ratio);
            }
        }

        private void FixedUpdate()
        {
            Simulate();
        }

        private void Simulate()
        {
            // SIMULATION
            for (int i = 1; i < segmentLength; i++)
            {
                RopeSegment firstSegment = ropeSegments[i];
                Vector2 velocity = firstSegment.posNow - firstSegment.posOld;
                firstSegment.posOld = firstSegment.posNow;
                firstSegment.posNow += velocity;
                firstSegment.posNow += forceGravity * Time.fixedDeltaTime;
                ropeSegments[i] = firstSegment;
            }

            //CONSTRAINTS
            for (int i = 0; i < 50; i++)
            {
                ApplyConstraint();
            }
        }

        private const float updateSpeed = 0.35f;

        private void ApplyConstraint()
        {
            //Constrant to First Point 
            RopeSegment firstSegment = ropeSegments[0];
            firstSegment.posNow = StartPoint;
            ropeSegments[0] = firstSegment;


            //Constrant to Second Point 
            RopeSegment endSegment = ropeSegments[^1];
            endSegment.posNow = EndPoint;
            ropeSegments[^1] = endSegment;

            for (int i = 0; i < segmentLength - 1; i++)
            {
                RopeSegment firstSeg = ropeSegments[i];
                RopeSegment secondSeg = ropeSegments[i + 1];

                float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
                float error = Mathf.Abs(dist - ropeSegLen);
                Vector2 changeDir = Vector2.zero;

                if (dist > ropeSegLen)
                {
                    changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
                }
                else if (dist < ropeSegLen)
                {
                    changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
                }

                Vector2 changeAmount = changeDir * error;
                if (i != 0)
                {
                    firstSeg.posNow -= changeAmount * updateSpeed;
                    ropeSegments[i] = firstSeg;
                    secondSeg.posNow += changeAmount * updateSpeed;
                    ropeSegments[i + 1] = secondSeg;
                }
                else
                {
                    secondSeg.posNow += changeAmount;
                    ropeSegments[i + 1] = secondSeg;
                }
            }
        }

        private void DrawRope()
        {
            float lineWidth = this.lineWidth;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;

            Vector3[] ropePositions = new Vector3[segmentLength];
            for (int i = 0; i < segmentLength; i++)
            {
                ropePositions[i] = ropeSegments[i].posNow;
            }

            lineRenderer.positionCount = ropePositions.Length;
            lineRenderer.SetPositions(ropePositions);
        }


        public struct RopeSegment
        {
            public Vector2 posNow;
            public Vector2 posOld;

            public RopeSegment(Vector2 pos)
            {
                posNow = pos;
                posOld = pos;
            }
        }
    }
}