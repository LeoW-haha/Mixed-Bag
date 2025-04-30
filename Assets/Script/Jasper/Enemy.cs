using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waypointThreshold = 0.1f;
    
    [Header("Combat Settings")]
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float knockbackForce = 5f;
    
    private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private bool isMoving = false;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // Add a circle collider for interactions
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<CircleCollider2D>();
            collider.radius = 0.3f;
            collider.isTrigger = true;
        }
    }

    private void Update()
    {
        if (!isMoving || waypoints == null || waypoints.Length == 0) return;

        // Move towards current waypoint
        Vector3 targetPosition = waypoints[currentWaypointIndex].position;
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Flip sprite based on movement direction
        if (spriteRenderer != null && moveDirection.x != 0)
        {
            spriteRenderer.flipX = moveDirection.x < 0;
        }

        // Check if reached waypoint
        if (Vector3.Distance(transform.position, targetPosition) < waypointThreshold)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Calculate knockback direction
                Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
                
                // Apply damage and knockback
                player.TakeDamage(damageAmount, knockbackDirection * knockbackForce);
            }
        }
    }

    public void Initialize(Transform[] pathWaypoints)
    {
        waypoints = pathWaypoints;
        if (waypoints != null && waypoints.Length > 0)
        {
            transform.position = waypoints[0].position;
            isMoving = true;
            currentWaypointIndex = 0;
        }
    }
} 