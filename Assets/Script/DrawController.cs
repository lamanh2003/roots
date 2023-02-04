using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawController : MonoBehaviour
{
    public DrawPoint drawPoint;

    private Vector3 mousePosition;
    private bool isDrawing;
    // Start is called before the first frame update
    void Start()
    {
        drawPoint.StopDraw();
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        drawPoint.SetPosition(new Vector3(mousePosition.x, mousePosition.y, 0f));

        if (Input.GetMouseButtonDown(0))
        {
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
