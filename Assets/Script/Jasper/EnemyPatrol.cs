using UnityEngine;
using System.Collections;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float waypointReachDistance = 0.1f;
    [SerializeField] private float startDelay = 0f;

    [Header("Animation")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite[] walkSprites;
    [SerializeField] private Sprite[] attackSprites;
    [SerializeField] private float animationSpeed = 0.2f;
    [SerializeField] private float attackDuration = 1f;

    [Header("Detection")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private string playerTag = "Player";

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private int orderInLayer = 1;

    private CircleCollider2D triggerCollider;
    private int currentWaypointIndex = 0;
    private bool isAttacking = false;
    private int currentSpriteIndex = 0;
    private float animationTimer = 0f;
    private bool isMoving = false;
    private bool canMove = false;

    private void Awake()
    {
        // Set up trigger collider with attackRange
        triggerCollider = GetComponent<CircleCollider2D>();
        if (triggerCollider == null)
        {
            triggerCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        triggerCollider.isTrigger = true;
        triggerCollider.radius = attackRange;
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on Enemy!");
            return;
        }

        spriteRenderer.sortingOrder = orderInLayer;
        if (defaultSprite != null)
        {
            spriteRenderer.sprite = defaultSprite;
        }

        StartCoroutine(StartPatrol());
    }

    private IEnumerator StartPatrol()
    {
        if (startDelay > 0)
        {
            yield return new WaitForSeconds(startDelay);
        }
        
        canMove = true;
    }

    private void Update()
    {
        if (!canMove || isAttacking || waypoints == null || waypoints.Length == 0) 
        {
            isMoving = false;
            return;
        }

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        if (targetWaypoint == null) return;

        // Move towards waypoint
        Vector2 direction = (targetWaypoint.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

        // Update animation
        isMoving = true;
        UpdateAnimation();

        // Flip sprite based on movement direction
        if (direction.x != 0)
        {
            spriteRenderer.flipX = direction.x < 0;
        }

        // Check if reached waypoint
        float distanceToWaypoint = Vector2.Distance(transform.position, targetWaypoint.position);
        if (distanceToWaypoint < waypointReachDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    private void UpdateAnimation()
    {
        if (walkSprites == null || walkSprites.Length == 0) return;

        if (isMoving)
        {
            // Update animation timer
            animationTimer += Time.deltaTime;
            if (animationTimer >= animationSpeed)
            {
                // Move to next sprite
                currentSpriteIndex = (currentSpriteIndex + 1) % walkSprites.Length;
                spriteRenderer.sprite = walkSprites[currentSpriteIndex];
                animationTimer = 0f;
            }
        }
        else
        {
            // Return to idle sprite
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

    private IEnumerator AnimateWalk()
    {
        while (true)
        {
            if (isMoving && !isAttacking && walkSprites != null && walkSprites.Length > 0)
            {
                // Play walk animation
                currentSpriteIndex = (currentSpriteIndex + 1) % walkSprites.Length;
                spriteRenderer.sprite = walkSprites[currentSpriteIndex];
            }
            else if (!isAttacking && idleSprite != null)
            {
                // Return to idle
                spriteRenderer.sprite = idleSprite;
            }
            
            yield return new WaitForSeconds(animationSpeed);
        }
    }

    private IEnumerator AttackAnimation()
    {
        isAttacking = true;
        
        // Play attack animation
        if (attackSprites != null && attackSprites.Length > 0)
        {
            for (int i = 0; i < attackSprites.Length; i++)
            {
                spriteRenderer.sprite = attackSprites[i];
                yield return new WaitForSeconds(attackDuration / attackSprites.Length);
            }
        }

        // Return to idle
        isAttacking = false;
        if (idleSprite != null)
        {
            spriteRenderer.sprite = idleSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && !isAttacking)
        {
            StartCoroutine(AttackAnimation());
            // The player's PlayerRespawn component will handle the death sequence automatically
        }
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;
            
            // Draw waypoint
            Gizmos.DrawWireSphere(waypoints[i].position, 0.3f);
            
            // Draw line to next waypoint
            if (i + 1 < waypoints.Length && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
            else if (i == waypoints.Length - 1 && waypoints[0] != null)
            {
                // Connect last waypoint to first
                Gizmos.DrawLine(waypoints[i].position, waypoints[0].position);
            }
        }
    }
} 