using System.Collections;
using System.Collections.Generic;
using Base;
using Controller;
using UnityEngine;
using UnityEngine.UI;
using Base;
public class DrawPoint : MonoBehaviour
{
    public TrailRenderer trail;
    public SpriteRenderer drawGfx;
    public ParticleSystem rippleFx;
    public Color color;

    private Transform trans;
    // Start is called before the first frame update
    void Start()
    {
        trans = transform;
    }
    public void Init()
    {
        StopDraw();
    }

    // Update is called once per frame
    void Update()
    {
        

    }
    public void SetPosition(Vector3 _position)
    {
        trans.position = _position;
    }
    public void StartDraw()
    {
        GamePlayController.Singleton.nodeController.listDrawNodes.Clear();

        rippleFx.Play();
        trans.gameObject.SetActive(true);
        UpdateColor(Node.LineColor.Pink);
    }
    public void StopDraw()
    {
        GamePlayController.Singleton.nodeController.UnHighlightAll();
        trans.gameObject.SetActive(false);
        
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
            collision.GetComponent<Node>().Highlight();
            GamePlayController.Singleton.nodeController.listDrawNodes.Add(collision.GetComponent<Node>());
        }
    }
}
