using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemCarousel : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool autoStart = true;
    
    [Header("Path Settings")]
    [SerializeField] private Transform[] pathPoints;
    [SerializeField] private float waypointReachDistance = 0.1f;
    
    private bool isSpawning = false;
    public bool pipeBurst = false;
    private OrderSystem orderSystem;
    [SerializeField] private int orderItemWeight; //Smaller = less likely
    private List<GameObject> activeItems = new List<GameObject>();

    private void Start()
    {
        // Validate setup
        if (itemPrefabs == null || itemPrefabs.Length == 0)
        {
            //Debug.LogError("ItemCarousel: No item prefabs assigned!");
            return;
        }

        if (pathPoints == null || pathPoints.Length == 0)
        {
            //Debug.LogError("ItemCarousel: No path points assigned!");
            return;
        }

        foreach (var prefab in itemPrefabs)
        {
            if (prefab == null)
            {
                //Debug.LogWarning("ItemCarousel: One or more item prefabs is null!");
            }
        }

        foreach (var point in pathPoints)
        {
            if (point == null)
            {
                //Debug.LogWarning("ItemCarousel: One or more path points is null!");
            }
        }

        if (autoStart)
        {
            //Debug.Log("ItemCarousel: Auto-starting spawn routine...");
            StartSpawning();
        }
        orderSystem = FindFirstObjectByType<OrderSystem>();
    }

    public void StartSpawning()
    {
        if (!isSpawning && itemPrefabs.Length > 0 && pathPoints.Length > 0)
        {
            //Debug.Log("ItemCarousel: Starting spawn routine");
            isSpawning = true;
            StartCoroutine(SpawnRoutine());
        }
    }

    public void StopSpawning()
    {
        //Debug.Log("ItemCarousel: Stopping spawn routine");
        isSpawning = false;
    }

    private IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            SpawnItem();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnItem()
    {
        if (pathPoints.Length == 0 || itemPrefabs.Length == 0) return;

        // Randomly select an item prefab
        int randomIndex = Random.Range(0, itemPrefabs.Length);
        GameObject itemPrefab = itemPrefabs[randomIndex];

        //Prioritizes items that are currently an order
        if (orderSystem != null) {
            if (itemPrefab.GetComponent<CollectibleItem>().GetItemName() != orderSystem.GetCurrentOrder().itemName || pipeBurst) {
                if (Random.Range(0,100) < orderItemWeight) {
                    randomIndex = Random.Range(0, itemPrefabs.Length);
                    itemPrefab = itemPrefabs[randomIndex];
                }
            }
        }

        if (itemPrefab == null)
        {
            //Debug.LogError("ItemCarousel: Selected prefab is null!");
            return;
        }

        // Spawn at first path point
        Vector3 spawnPosition = pathPoints[0].position;
        GameObject newItem = Instantiate(itemPrefab, spawnPosition, Quaternion.identity, transform);
        //Debug.Log($"ItemCarousel: Spawned item {newItem.name} at position {spawnPosition}");
        
        // Set up item movement
        ItemMover mover = newItem.AddComponent<ItemMover>();
        mover.Initialize(pathPoints, moveSpeed, waypointReachDistance);
        
        // Add to active items list
        activeItems.Add(newItem);
        //Debug.Log($"ItemCarousel: Active items count: {activeItems.Count}");
    }

    public void RemoveItem(GameObject item)
    {
        if (activeItems.Contains(item))
        {
            activeItems.Remove(item);
            Destroy(item);
            //Debug.Log($"ItemCarousel: Removed item. Active items remaining: {activeItems.Count}");
        }
    }

    private void OnDrawGizmos()
    {
        if (pathPoints == null || pathPoints.Length < 2) return;

        // Draw path in editor
        Gizmos.color = Color.cyan;
        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            if (pathPoints[i] != null && pathPoints[i + 1] != null)
            {
                Gizmos.DrawLine(pathPoints[i].position, pathPoints[i + 1].position);
                Gizmos.DrawWireSphere(pathPoints[i].position, waypointReachDistance);
            }
        }
        
        // Draw last point
        if (pathPoints[pathPoints.Length - 1] != null)
        {
            Gizmos.DrawWireSphere(pathPoints[pathPoints.Length - 1].position, waypointReachDistance);
        }
    }
}

// Helper component to move items along the path
public class ItemMover : MonoBehaviour
{
    private Transform[] pathPoints;
    private float moveSpeed;
    private float reachDistance;
    private int currentPointIndex = 0;
    private CollectibleItem collectibleItem;
    private bool hasBeenPickedUp = false;

    public void Initialize(Transform[] points, float speed, float wayPointReachDistance)
    {
        pathPoints = points;
        moveSpeed = speed;
        reachDistance = wayPointReachDistance;
        collectibleItem = GetComponent<CollectibleItem>();
        //Debug.Log($"ItemMover: Initialized for {gameObject.name} with {points.Length} points");
    }

    private void Update()
    {
        if (pathPoints == null || pathPoints.Length == 0) return;
        
        // If the item has ever been picked up, stop following the path
        if (hasBeenPickedUp) return;
        
        // Check if item is currently being carried
        if (collectibleItem != null)
        {
            if (collectibleItem.IsPickedUp())
            {
                hasBeenPickedUp = true;
                return;
            }
        }

        // Move towards current point
        Transform currentPoint = pathPoints[currentPointIndex];
        Vector3 direction = (currentPoint.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Check if reached current point
        float distanceToPoint = Vector2.Distance(transform.position, currentPoint.position);
        if (distanceToPoint < reachDistance)
        {
            // Move to next point or destroy if at end
            currentPointIndex++;
            if (currentPointIndex >= pathPoints.Length)
            {
                // Get carousel and remove item
                ItemCarousel carousel = GetComponentInParent<ItemCarousel>();
                if (carousel != null)
                {
                    carousel.RemoveItem(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                //Debug.Log($"ItemMover: {gameObject.name} reached point {currentPointIndex-1}, moving to point {currentPointIndex}");
            }
        }
    }
} 