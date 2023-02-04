using System.Collections;
using System.Collections.Generic;
using Base;
using Controller;
using UnityEngine;
using UnityEngine.UI;
using Base;
using Utilities;

public class DrawPoint : MonoBehaviour
{
    public AudioClip touchLineSfx;
    public TrailRenderer trail;
    public SpriteRenderer drawGfx;
    public ParticleSystem rippleFx;
    public Color color;

    private List<Vector3> points;
    // Start is called before the first frame update
    void Start()
    {
        points = new List<Vector3>();
        StopDraw();
    }
    public void Init()
    {
    }

    // Update is called once per frame
    void Update()
    {
        

    }
    public void SetPosition(Vector3 _position)
    {
        transform.position = _position;
    }
    public void StartDraw()
    {
        transform.gameObject.SetActive(true);

        GamePlayController.Singleton.nodeController.listDrawNodes.Clear();

        rippleFx.Play();
        UpdateColor(Node.LineColor.Pink);
    }
    public void StopDraw()
    {

        GamePlayController.Singleton.nodeController.UnHighlightAll();
        if (GamePlayController.Singleton.nodeController.CheckTurn())
        {
            foreach (var nTmp in GamePlayController.Singleton.nodeController.listDrawNodes)
            {
                GamePlayController.Singleton.nodeController.ChangeNodeColor(nTmp);
            }
            GamePlayController.Singleton.nodeController.ClaimAll();
            var allChild = GamePlayController.Singleton.nodeController.rootNode.AllFarestNode;
            allChild.Shuffle();
            int left = GamePlayController.Singleton.levelController.config.growNumber <= GamePlayController.Singleton.nodeController.rootNode.ChildCount
                ? GamePlayController.Singleton.levelController.config.growNumber
                : GamePlayController.Singleton.nodeController.rootNode.ChildCount;
            int i = 0;
            while (left-->0)
            {
                if (allChild[i].CompareTag("rootNode"))
                {
                    left++;
                    continue;
                }
                GamePlayController.Singleton.nodeController.AddNode(allChild[i]);
                i++;
            }
        }
        transform.gameObject.SetActive(false);
    }
    private void UpdateColor(Node.LineColor _color)
    {
        color = LineColorExtension.ToColor(_color);

        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );

        drawGfx.color = color;
        trail.colorGradient = gradient;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag != "rootNode")
        {
            Node _node = collision.GetComponent<Node>();
            if(_node.isConnected)
            {
                return;
            }
            _node.isConnected = true;
            GameController.Singleton.soundController.PlaySound(touchLineSfx);
            _node.Highlight();
            GamePlayController.Singleton.nodeController.listDrawNodes.Add(_node);
        }
    }
}
