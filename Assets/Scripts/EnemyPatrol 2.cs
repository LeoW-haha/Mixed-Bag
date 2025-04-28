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
    [SerializeField] private float animationFrameRate = 0.2f;
    [SerializeField] private float attackDuration = 1f;

    [Header("Detection")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private string playerTag = "Player";

    private SpriteRenderer spriteRenderer;
    private CircleCollider2D triggerCollider;
    private int currentWaypointIndex = 0;
    private bool isAttacking = false;
    private int currentWalkFrame = 0;
    private bool isMoving = false;
    private bool canMove = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (idleSprite != null)
        {
            spriteRenderer.sprite = idleSprite;
        }

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
        StartCoroutine(StartPatrol());
    }

    private IEnumerator StartPatrol()
    {
        if (startDelay > 0)
        {
            yield return new WaitForSeconds(startDelay);
        }
        
        canMove = true;
        StartCoroutine(AnimateWalk());
    }

    private void Update()
    {
        if (!canMove || isAttacking || waypoints == null || waypoints.Length == 0) 
        {
            isMoving = false;
            return;
        }

        // Move towards current waypoint
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        if (targetWaypoint == null) return;

        Vector2 direction = (targetWaypoint.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

        // Set moving state
        isMoving = direction.magnitude > 0.01f;

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

    private IEnumerator AnimateWalk()
    {
        while (true)
        {
            if (isMoving && !isAttacking && walkSprites != null && walkSprites.Length > 0)
            {
                // Play walk animation
                currentWalkFrame = (currentWalkFrame + 1) % walkSprites.Length;
                spriteRenderer.sprite = walkSprites[currentWalkFrame];
            }
            else if (!isAttacking && idleSprite != null)
            {
                // Return to idle
                spriteRenderer.sprite = idleSprite;
            }
            
            yield return new WaitForSeconds(animationFrameRate);
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
} 