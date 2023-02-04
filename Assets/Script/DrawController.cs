using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controller;
public class DrawController : MonoBehaviour
{
    public DrawPoint drawPoint;

    private Vector3 mousePosition;
    private bool isDrawing;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (isDrawing)
        {
            drawPoint.SetPosition(new Vector3(mousePosition.x, mousePosition.y, 0f));
        }
        if (Input.GetMouseButtonDown(0))
        {
            GameController.Singleton.soundController.PlayClickSound();
            isDrawing = true;
            drawPoint.StartDraw();
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
            drawPoint.StopDraw();
        }
    }
}
