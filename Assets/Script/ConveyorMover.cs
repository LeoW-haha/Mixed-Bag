using UnityEngine;
using System.Collections.Generic;

public class ConveyorMover : MonoBehaviour
{
    public List<Transform> waypoints;
    public float speed = 2f;
    private int currentIndex = 0;

    void Update()
    {
        if (waypoints == null || currentIndex >= waypoints.Count) return;

        Transform target = waypoints[currentIndex];
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Check if close enough to consider as 'reached'
        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            currentIndex++;

            if (currentIndex >= waypoints.Count)
            {
                // Stop moving
                enabled = false;
            }
        }
    }

    public void SetPath(Transform wp1, Transform wp2)
    {
        waypoints = new List<Transform> { wp1, wp2 };
        currentIndex = 0;
    }
}
