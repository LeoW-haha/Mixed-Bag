using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float deathAnimationDuration = 1.5f; // Adjust this to match your animation length
    
    private Rigidbody2D rb;
    private PlayerAnimator playerAnimator;
    private Vector2 movement;
    private bool isDead = false;
    private bool isRespawning = false;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<PlayerAnimator>();
        
        // Configure Rigidbody2D
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.gravityScale = 0;
    }
    
    private void Update()
    {
        if (isDead || isRespawning) return;

        // Input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;
        
        // Update animations
        if (playerAnimator != null)
        {
            playerAnimator.UpdateAnimation(movement);
        }
    }
    
    private void FixedUpdate()
    {
        if (isDead || isRespawning) return;
        
        // Movement
        rb.linearVelocity = movement * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check for enemy collision
        if (other.CompareTag("Enemy") && !isDead && !isRespawning)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        isRespawning = false;
        rb.linearVelocity = Vector2.zero;
        
        if (playerAnimator != null)
        {
            playerAnimator.PlayDeathAnimation();
        }

        // Wait for death animation to finish before respawning
        Invoke("StartRespawn", deathAnimationDuration);
    }

    private void StartRespawn()
    {
        isRespawning = true;
        isDead = false;
        
        // Reset position
        transform.position = Vector3.zero; // Or your designated spawn point
        
        // Reset velocity
        rb.linearVelocity = Vector2.zero;
        movement = Vector2.zero;
        
        // Reset animator
        if (playerAnimator != null)
        {
            playerAnimator.ResetToIdle();
        }
        
        // Short delay before allowing movement again
        Invoke("CompleteRespawn", 0.1f);
    }

    private void CompleteRespawn()
    {
        isRespawning = false;
    }
} 