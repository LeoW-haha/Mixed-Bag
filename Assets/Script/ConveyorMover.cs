using UnityEngine;
using System.Collections.Generic;

public class ConveyorMover : MonoBehaviour
{
    public List<Transform> waypoints;
    public float speed = 2f;
    private int currentIndex = 0;

    private bool IsDelivery => gameObject.name.Contains("Package") || CompareTag("Delivery");

    void Update()
    {
        if (waypoints == null || waypoints.Count == 0 || currentIndex >= waypoints.Count) return;

        Transform target = waypoints[currentIndex];
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            currentIndex++;

            if (currentIndex >= waypoints.Count)
            {
                if (IsDelivery)
                    Destroy(gameObject);
                else
                    enabled = false;
            }
        }
    }

    public void SetPath(Transform wp1, Transform wp2)
    {
        if (wp1 == null || wp2 == null) return;

        waypoints = new List<Transform> { wp1, wp2 };
        currentIndex = 0;
    }
}
