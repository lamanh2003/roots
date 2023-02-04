//https://www.youtube.com/@jasony4017
using System;
using System.Collections;
using System.Collections.Generic;
using Base;
using DG.Tweening;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utilities
{
    [RequireComponent(typeof(LineRenderer))]
    public class RopeBridge : MonoBehaviour
    {
        public Transform StartPoint;
        public Transform EndPoint;

        [HideInInspector] public LineRenderer lineRenderer;
        private NativeArray<RopeSegment> ropeSegments,ropeSegments2;
        private NativeArray<float2> colPoints;
        private float ropeSegLen = 0.2f;
        private int segmentLength = 5;
        private float lineWidth = 0.1f;
        private float2 forceGravity;
        private float2 startGravity;
        private float2 endGravity;
        private float gravityChangeTime;
        private float gravityTotalChangeTime;

        [SerializeField] private Material defaultMat;

        private JobHandle simulateHandle;
        private SimulateJob _simulateJob;
        private float twoFrameTime = 0;
        
        private struct SimulateJob: IJob
        {
            public float2 startPoint, endPoint;
            public int segmentLength;
            public float ropeSegLen;
            public float2 forceGravity;
            public NativeArray<RopeSegment> ropeSegments;
            public float time;
            public void Execute()
            {
                // SIMULATION
                for (int i = 1; i < segmentLength; i++)
                {
                    RopeSegment firstSegment = ropeSegments[i];
                    float2 velocity = firstSegment.posNow - firstSegment.posOld;
                    firstSegment.posOld = firstSegment.posNow;
                    firstSegment.posNow += velocity;
                    firstSegment.posNow += forceGravity * time;
                    ropeSegments[i] = firstSegment;
                }

                //CONSTRAINTS
                for (int i = 0; i < 50; i++)
                {
                    RopeSegment firstSegment = ropeSegments[0];
                    firstSegment.posNow = startPoint;
                    ropeSegments[0] = firstSegment;


                    //Constrant to Second Point 
                    RopeSegment endSegment = ropeSegments[^1];
                    endSegment.posNow = endPoint;
                    ropeSegments[^1] = endSegment;

                    for (int j = 0; j < segmentLength - 1; j++)
                    {
                        RopeSegment firstSeg = ropeSegments[j];
                        RopeSegment secondSeg = ropeSegments[j + 1];
                        
                        float dist = math.length(firstSeg.posNow - secondSeg.posNow);
                        float error = math.abs(dist - ropeSegLen);
                        float2 changeDir = float2.zero;

                        if (dist > ropeSegLen)
                        {
                            if (math.distance(firstSeg.posNow,secondSeg.posNow)<0.1f)
                            {
                                changeDir = float2.zero;
                            }else
                            {
                                changeDir = math.normalize(firstSeg.posNow - secondSeg.posNow);
                            }
                        }
                        else if (dist < ropeSegLen)
                        {
                            if (math.distance(secondSeg.posNow,firstSeg.posNow)<0.1f)
                            {
                                changeDir = float2.zero;
                            }else
                            {
                                changeDir = math.normalize(secondSeg.posNow - firstSeg.posNow);
                            }
                        }

                        float2 changeAmount = changeDir * error;
                        if (j != 0)
                        {
                            firstSeg.posNow -= changeAmount * 0.55f;
                            ropeSegments[j] = firstSeg;
                            secondSeg.posNow += changeAmount * 0.55f;
                            ropeSegments[j + 1] = secondSeg;
                        }
                        else
                        {
                            secondSeg.posNow += changeAmount;
                            ropeSegments[j + 1] = secondSeg;
                        }
                    }
                }
            }
        }

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
                float gravityScale = 1f;
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
                if (math.distance(startGravity,endGravity) <= 0.01f)
                {
                    continue;
                }
                gravityChangeTime = 0f;
                gravityTotalChangeTime = Random.Range(1f, 3f);
                yield return new WaitForSeconds(gravityTotalChangeTime);
            }
        }
        

        public void Init(Transform startPoint, Transform endPoint, Node.LineColor lineColor)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            ropeSegments = new NativeArray<RopeSegment>(segmentLength,Allocator.Persistent);
            ropeSegments2 = new NativeArray<RopeSegment>(segmentLength,Allocator.Persistent);
            colPoints = new NativeArray<float2>(segmentLength,Allocator.Persistent);
            StartCoroutine(ChangeGravityForce());
            lineRenderer.startColor = lineColor.ToColor();
            lineRenderer.endColor = lineRenderer.startColor;
            float2 ropeStartPoint = new float2(StartPoint.position.x,StartPoint.position.y) ;
            

            for (int i = 0; i < segmentLength; i++)
            {
                ropeSegments[i]=new RopeSegment(ropeStartPoint);
                colPoints[i]=ropeStartPoint;
                ropeStartPoint.y -= ropeSegLen;
            }

            _simulateJob = new SimulateJob()
            {
                startPoint = new float2(StartPoint.position.x, StartPoint.position.y),
                endPoint = new float2(EndPoint.position.x, EndPoint.position.y),
                forceGravity = forceGravity,
                ropeSegLen = ropeSegLen,
                ropeSegments = ropeSegments2,
                segmentLength = segmentLength
            };
            //col.SetPoints(colPoints);
        }



        // Update is called once per frame
        void Update()
        {
            gravityChangeTime += Time.deltaTime;
            forceGravity = Vector2.Lerp(startGravity, endGravity, gravityChangeTime / gravityTotalChangeTime);
            DrawRope();
        }

        private void FixedUpdate()
        {
            if (simulateHandle.IsCompleted)
            {
                simulateHandle.Complete();
                _simulateJob.startPoint = new float2(StartPoint.position.x, StartPoint.position.y);
                _simulateJob.endPoint = new float2(EndPoint.position.x, EndPoint.position.y);
                _simulateJob.time = twoFrameTime;
                _simulateJob.forceGravity = forceGravity;
                twoFrameTime = 0;
                ropeSegments.CopyFrom(ropeSegments2);
                simulateHandle = _simulateJob.Schedule();
            }
            else
            {
                twoFrameTime += Time.fixedDeltaTime;
            }
            //Simulate();
        }

        /*private void Simulate()
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

        private const float updateSpeed = 0.35f;*/

        /*private void ApplyConstraint()
        {
            //Constrant to First Point 
            RopeSegment firstSegment = ropeSegments[0];
            firstSegment.posNow = StartPoint.position;
            ropeSegments[0] = firstSegment;


            //Constrant to Second Point 
            RopeSegment endSegment = ropeSegments[^1];
            endSegment.posNow = EndPoint.position;
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
        }*/

        private void DrawRope()
        {
            float lineWidth = this.lineWidth;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;

            Vector3[] ropePositions = new Vector3[segmentLength];
            for (int i = 0; i < segmentLength; i++)
            {
                ropePositions[i] =  new Vector3(ropeSegments[i].posNow.x,ropeSegments[i].posNow.y,0f);
            }

            lineRenderer.positionCount = ropePositions.Length;
            lineRenderer.SetPositions(ropePositions);
        }


        public struct RopeSegment
        {
            public float2 posNow;
            public float2 posOld;

            public RopeSegment(Vector2 pos)
            {
                posNow = pos;
                posOld = pos;
            }
        }

        public void Fade(float time)
        {
            lineRenderer.material = defaultMat;
            var end = lineRenderer.startColor;
            end.a = 0;
            lineRenderer.DOColor(new Color2(lineRenderer.startColor,lineRenderer.endColor), new Color2(end,end), time);
        }
    }
}