using UnityEngine;

public class CameraZoomIn : MonoBehaviour
{
    public float startSize = 20f;      // The size to start zoomed out at
    public float targetSize = 7f;      // The size to zoom in to (your current value)
    public float zoomDuration = 2f;    // How long the zoom takes (seconds)

    private Camera cam;
    private float timer = 0f;
    private bool zooming = true;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.orthographicSize = startSize;
        }
    }

    void Update()
    {
        if (!zooming || cam == null) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / zoomDuration);
        cam.orthographicSize = Mathf.Lerp(startSize, targetSize, t);

        if (t >= 1f)
        {
            zooming = false;
        }
    }
} 