using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waypointThreshold = 0.1f;
    
    [Header("Combat Settings")]
    [SerializeField] private float damageAmount = 34f;
    [SerializeField] private float knockbackForce = 5f;
    
    [Header("Animation")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite[] walkSprites;
    [SerializeField] private float animationSpeed = 0.2f;
    
    private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private bool isMoving = false;
    private SpriteRenderer spriteRenderer;
    private int currentSpriteIndex = 0;
    private float animationTimer = 0f;
    private bool isRainbowMode = false;
    private float originalMoveSpeed;
    private Coroutine rainbowCoroutine;
    private float gameDuration = 180f; // 3 minutes in seconds

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
        if (!isMoving || waypoints == null || waypoints.Length == 0) {
            if (!isMoving) //Debug.Log("[Enemy] Not moving (isMoving is false)");
            if (waypoints == null) //Debug.Log("[Enemy] Not moving (waypoints is null)");
            if (waypoints != null && waypoints.Length == 0) //Debug.Log("[Enemy] Not moving (waypoints is empty)");
            return;
        }

        // Rainbow mode logic
        float timeLeft = gameDuration - Time.time;
        if (!isRainbowMode && timeLeft <= 60f) // Last 1 minute
        {
            isRainbowMode = true;
            moveSpeed = originalMoveSpeed * 2f;
            if (rainbowCoroutine == null)
                rainbowCoroutine = StartCoroutine(RainbowFlash());
        }

        Vector3 targetPosition = waypoints[currentWaypointIndex].position;
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Animation logic
        UpdateAnimation();

        //Debug.Log($"[Enemy] Moving towards waypoint {currentWaypointIndex} at {targetPosition}, current position: {transform.position}");

        // Flip sprite based on movement direction
        if (spriteRenderer != null && moveDirection.x != 0)
        {
            spriteRenderer.flipX = moveDirection.x > 0;
        }

        // Check if reached waypoint
        if (Vector3.Distance(transform.position, targetPosition) < waypointThreshold)
        {
            if (currentWaypointIndex >= waypoints.Length - 1)
            {
                // Last waypoint reached, destroy this robot
                Destroy(gameObject);
                return;
            }
            else
            {
                currentWaypointIndex++;
                //Debug.Log($"[Enemy] Reached waypoint {currentWaypointIndex}, moving to next.");
            }
        }
    }

    private void UpdateAnimation()
    {
        if (walkSprites == null || walkSprites.Length == 0 || spriteRenderer == null) return;

        if (isMoving)
        {
            animationTimer += Time.deltaTime;
            if (animationTimer >= animationSpeed)
            {
                currentSpriteIndex = (currentSpriteIndex + 1) % walkSprites.Length;
                spriteRenderer.sprite = walkSprites[currentSpriteIndex];
                animationTimer = 0f;
            }
        }
        else
        {
            if (idleSprite != null)
            {
                spriteRenderer.sprite = idleSprite;
            }
            else
            {
                spriteRenderer.sprite = walkSprites[0];
            }
            currentSpriteIndex = 0;
            animationTimer = 0f;
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
            originalMoveSpeed = moveSpeed;
            //Debug.Log($"[Enemy] Initialized with {waypoints.Length} waypoints. Starting at {waypoints[0].position}");
        }
        else
        {
            //Debug.LogWarning("[Enemy] Initialize called with null or empty waypoints!");
        }
    }

    private System.Collections.IEnumerator RainbowFlash()
    {
        Color[] rainbowColors = new Color[]
        {
            Color.red,
            new Color(1f, 0.5f, 0f), // Orange
            Color.yellow,
            Color.green,
            Color.cyan,
            Color.blue,
            new Color(0.5f, 0f, 1f), // Indigo/Violet
            Color.magenta
        };

        int colorIndex = 0;
        while (true)
        {
            spriteRenderer.color = rainbowColors[colorIndex];
            colorIndex = (colorIndex + 1) % rainbowColors.Length;
            yield return new WaitForSeconds(0.15f);
        }
    }
} 