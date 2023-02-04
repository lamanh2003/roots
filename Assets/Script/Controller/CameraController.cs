using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 10f;

    public float panBorderThickness = 1.5f;
    // Start is called before the first frame update
    private void Update()
    {
        Vector3 pos = transform.position;
        if (Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.y += panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y <= panBorderThickness)
        {
            pos.y -= panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.x += panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x <= panBorderThickness)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }
        
        pos.z = -10f;
        transform.position = pos;
    }
}
