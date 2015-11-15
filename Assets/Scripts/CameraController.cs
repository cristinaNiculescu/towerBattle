using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{

    [Tooltip("Camera move speed")]
    [Range(1, 10)]
    public float speed = 3;
    [Tooltip("Change how much zoom each 'tick' with the mousewheel does")]
    [Range(30, 200)]
    public float zoomRate = 50;
    [Tooltip("how much can you zoom in")]
    [Range(1, 99)]
    public int closestZoom = 10;
    [Tooltip("how much can you zoom out")]
    [Range(100, 300)]
    public int furthestZoom = 200;

    private Camera cam;
    private float size;

    private float dragSpeed = 150;
    private Vector3 dragOrigin;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {

    }

    void LateUpdate()
    {
        Zoom();
        DragCameraMiddleMouse();
    }

    /// <summary>
    /// Draggin the camera around while holding down the mousewheel button.
    /// </summary>
    void DragCameraMiddleMouse()
    {
        if (Input.GetMouseButtonDown(2))
        {
            dragOrigin = Input.mousePosition;
            return;
        }
        if (Input.GetMouseButton(2))
        {
            Vector3 pos = cam.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            //Move the direction we are pulling
            Vector3 move = new Vector3(pos.x, 0f, pos.y);
            //Set the Move direction to a negative value, since we want a inverted direction drag.
            cam.transform.Translate(-move * Time.deltaTime * dragSpeed, Space.World);
        }
    }

    /// <summary>
    /// Zoom the orthograhic camera size by using the mousewheel
    /// </summary>
    void Zoom()
    {
        if ((Input.GetAxis("Mouse ScrollWheel") != 0f))
        {
            size = Mathf.Clamp(size - (Input.GetAxis("Mouse ScrollWheel") * zoomRate), closestZoom, furthestZoom);
            cam.orthographicSize = size;
        }
    }
}
