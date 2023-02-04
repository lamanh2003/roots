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

        private LineRenderer lineRenderer;
        private List<RopeSegment> ropeSegments;
        private float ropeSegLen = 0.05f;
        private int segmentLength = 20;
        private float lineWidth = 0.1f;
        private Vector2 forceGravity;
        private Vector2 startGravity;
        private Vector2 endGravity;
        private float gravityChangeTime;

        private int indexMousePos;

        [SerializeField] private Vector2 followTarget;

        [SerializeField] private float followTaretUpdateSpeed;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        private IEnumerator ChangeGravityForce()
        {
            while (true)
            {
                int rand = Random.Range(0, 4);
                startGravity = forceGravity;
                Vector2 tmpGravity = Vector2.zero;
                float gravityScale = 0.8f;
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

                gravityChangeTime = 0f;
                endGravity = tmpGravity * gravityScale;
                yield return new WaitForSeconds(2f);
            }
        }

        private IEnumerator ChangeHeavyPoint()
        {
            while (true)
            {
                float rootScale = followTaretUpdateSpeed >= 0 ? 1 : -1;
                float rand = Random.Range(0.4f, 6f) * (Random.Range(0, 2) == 0 ? -1 : 1);
                float scale = 0.01f;
                followTaretUpdateSpeed = rand * scale * rootScale;
                yield return new WaitForSeconds((1f) / followTaretUpdateSpeed);
            }
        }

        public void Init(Vector2 startPoint, Vector2 endPoint, Node.LineColor lineColor)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            dirVec = (EndPoint - StartPoint).normalized;
            ropeSegments = new List<RopeSegment>();
            StartCoroutine(ChangeGravityForce());
            StartCoroutine(ChangeHeavyPoint());
            lineRenderer.startColor = lineColor.ToColor();
            lineRenderer.endColor = lineRenderer.startColor;
            Vector3 ropeStartPoint = StartPoint;

            for (int i = 0; i < segmentLength; i++)
            {
                ropeSegments.Add(new RopeSegment(ropeStartPoint));
                ropeStartPoint.y -= ropeSegLen;
            }
        }


        // Update is called once per frame
        void Update()
        {
            gravityChangeTime += Time.deltaTime;
            forceGravity = Vector2.Lerp(startGravity, endGravity, gravityChangeTime / 2f);
            followTarget += dirVec * (followTaretUpdateSpeed * Time.deltaTime);
            if (Vector2.Distance(followTarget,StartPoint) >=1f || Vector2.Distance(followTarget,EndPoint) >=1f)
            {
                followTaretUpdateSpeed *= -1;
            }
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
                    firstSeg.posNow -= changeAmount * 0.5f;
                    ropeSegments[i] = firstSeg;
                    secondSeg.posNow += changeAmount * 0.5f;
                    ropeSegments[i + 1] = secondSeg;
                }
                else
                {
                    secondSeg.posNow += changeAmount;
                    ropeSegments[i + 1] = secondSeg;
                }

                //Heavy point
                if (indexMousePos > 0 && indexMousePos < this.segmentLength - 1 && i == indexMousePos)
                {
                    RopeSegment segment = ropeSegments[i];
                    RopeSegment segment2 = ropeSegments[i + 1];
                    segment.posNow = new Vector2(followTarget.x, followTarget.y);
                    segment2.posNow = new Vector2(followTarget.x, followTarget.y);
                    ropeSegments[i] = segment;
                    ropeSegments[i + 1] = segment2;
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