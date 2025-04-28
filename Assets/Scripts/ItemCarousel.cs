using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemCarousel : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float waypointReachDistance = 0.1f;
    [SerializeField] private float startDelay = 0f;

    private Transform itemsContainer;
    private Vector3 containerStartPosition;

    private void Start()
    {
        // Find or create the Items container
        itemsContainer = transform.Find("Items");
        if (itemsContainer == null)
        {
            GameObject items = new GameObject("Items");
            items.transform.parent = transform;
            itemsContainer = items.transform;
        }

        // Store the initial position
        containerStartPosition = itemsContainer.position;

        // Start each item with its own delay
        for (int i = 0; i < itemsContainer.childCount; i++)
        {
            Transform item = itemsContainer.GetChild(i);
            StartCoroutine(StartItemMovement(item, i * 2f)); // 2 second delay between each item
        }
    }

    private IEnumerator StartItemMovement(Transform item, float delay)
    {
        // Wait for delay
        yield return new WaitForSeconds(startDelay + delay);
        
        int itemWaypointIndex = 0;
        
        while (true)
        {
            if (waypoints == null || waypoints.Length == 0) yield break;

            Transform targetWaypoint = waypoints[itemWaypointIndex];
            if (targetWaypoint == null) yield break;

            // Move towards current waypoint
            while (Vector2.Distance(item.position, targetWaypoint.position) > waypointReachDistance)
            {
                // If item is picked up, stop tracking it
                CollectibleItem collectible = item.GetComponent<CollectibleItem>();
                if (collectible != null && collectible.IsPickedUp())
                {
                    yield break;
                }
                
                Vector2 direction = (targetWaypoint.position - item.position).normalized;
                item.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
                yield return null;
            }

            // Move to next waypoint
            itemWaypointIndex = (itemWaypointIndex + 1) % waypoints.Length;
            yield return null;
        }
    }

    // Optional: Visualize the path in the editor
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.yellow;
        
        // Draw lines between waypoints
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;
            
            // Draw a sphere at the waypoint
            Gizmos.DrawWireSphere(waypoints[i].position, 0.2f);
            
            // Draw line to next waypoint
            if (i + 1 < waypoints.Length && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
            // Draw line from last to first waypoint
            else if (i == waypoints.Length - 1 && waypoints[0] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[0].position);
            }
        }
    }
} 