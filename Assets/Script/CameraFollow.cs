using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // The player to follow
    [SerializeField] private float smoothSpeed = 0.125f; // Lower = smoother
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f); // Camera offset from player

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;
        
        // Smoothly move camera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        
        // Keep the z-offset constant
        smoothedPosition.z = offset.z;
        
        // Update camera position
        transform.position = smoothedPosition;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
} 