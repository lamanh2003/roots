using System.Collections;
using System.Collections.Generic;
using Base;
using Controller;
using UnityEngine;
using UnityEngine.UI;
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
        rippleFx.Play();
        trans.gameObject.SetActive(true);
        UpdateColor(Color.red);
    }
    public void StopDraw()
    {
        trans.gameObject.SetActive(false);
    }
    private void UpdateColor(Color32 _color)
    {
        color = _color;

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
        Debug.LogError("touch line");
        
        GamePlayController.Singleton.nodeController.ChangeNodeColor(collision.GetComponent<Node>());
    }
}
