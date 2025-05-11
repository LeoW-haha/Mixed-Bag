using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("Item Handling")]
    [SerializeField] private Transform carryPoint; // Empty GameObject as a child of player
    [SerializeField] private float pickupCooldown = 0.5f;
    
    [Header("Combat Settings")]
    [SerializeField] private float invincibilityDuration = 1f;
    [SerializeField] private float knockbackDuration = 0.2f;
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private Image HealthBar;
    [SerializeField] private float respawnDelay = 2f; // Time before respawning
    
    private Rigidbody2D rb;
    private PlayerAnimator playerAnimator;
    private GameObject carriedItem;
    private float lastPickupTime;
    private bool isDead = false;
    private Vector2 movement;
    private float currentHealth;
    private float invincibilityTimer = 0f;
    private float knockbackTimer = 0f;
    private Vector2 knockbackVelocity;
    private Vector3 startPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<PlayerAnimator>();
        currentHealth = maxHealth;
        
        // Store starting position
        startPosition = transform.position;
        
        // Create carry point if not set
        if (carryPoint == null)
        {
            GameObject carryPointObj = new GameObject("CarryPoint");
            carryPoint = carryPointObj.transform;
            carryPoint.SetParent(transform);
            carryPoint.localPosition = new Vector3(0, 0.5f, 0); // Adjust position as needed
        }
        
        // Configure Rigidbody2D
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.gravityScale = 0;
    }

    private void Update()
    {

        HealthBar.fillAmount = currentHealth/maxHealth;

        if (isDead) return;

        // Update invincibility
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }

        // Get input (disabled during knockback)
        if (knockbackTimer <= 0)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            
            // Normalize diagonal movement
            if (movement.magnitude > 1f)
            {
                movement.Normalize();
            }
        }
        else
        {
            knockbackTimer -= Time.deltaTime;
        }

        // Update animation
        if (playerAnimator != null)
        {
            playerAnimator.UpdateAnimation(movement);
        }

        // Handle item drop with Q key
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropItem();
        }
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        
        // Apply movement or knockback
        if (knockbackTimer > 0)
        {
            rb.linearVelocity = knockbackVelocity;
        }
        else
        {
            float speedMultiplier = 1f;
            if (carriedItem != null)
            {
                CollectibleItem collectible = carriedItem.GetComponent<CollectibleItem>();
                if (collectible != null)
                    speedMultiplier = collectible.carrySpeedMultiplier;
            }
            rb.linearVelocity = movement * moveSpeed * speedMultiplier;
        }
    }

    public bool CanPickupItem()
    {
        return carriedItem == null && Time.time > lastPickupTime + pickupCooldown && !isDead;
    }

    public void PickupItem(GameObject item)
    {
        if (!CanPickupItem()) return;

        carriedItem = item;
        
        // Call the item's pickup method
        CollectibleItem collectible = item.GetComponent<CollectibleItem>();
        if (collectible != null)
        {
            collectible.PickUp(transform);
        }
        
        // Update animation state
        if (playerAnimator != null)
        {
            playerAnimator.SetCarrying(true);
        }
        
        lastPickupTime = Time.time;
    }

    public void DropItem()
    {
        if (carriedItem == null) return;

        // Call the item's drop method
        CollectibleItem collectible = carriedItem.GetComponent<CollectibleItem>();
        if (collectible != null)
        {
            collectible.Drop();
        }
        
        // Position slightly in front of player if moving, otherwise just at player position
        Vector3 dropPosition = transform.position;
        if (movement.magnitude > 0.1f)
        {
            dropPosition += (Vector3)movement.normalized;
        }
        carriedItem.transform.position = dropPosition;
        
        carriedItem = null;
        
        // Update animation state
        if (playerAnimator != null)
        {
            playerAnimator.SetCarrying(false);
        }
    }

    public void Die()
    {
        if (isDead) return;
        
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        currentHealth = 0;
        
        // Drop item if carrying one
        if (carriedItem != null)
        {
            DropItem();
        }
        
        // Play death animation and respawn after delay
        if (playerAnimator != null)
        {
            playerAnimator.PlayDeathAnimation();
            StartCoroutine(RespawnAfterDelay());
        }
        else
        {
            StartCoroutine(RespawnAfterDelay());
        }
    }

    private IEnumerator RespawnAfterDelay()
    {
        // Wait for death animation and delay
        yield return new WaitForSeconds(respawnDelay);
        
        // Reset position and health
        transform.position = startPosition;
        currentHealth = maxHealth;
        isDead = false;

        // Deduct 200 points for respawn
        if (GameManagerJasper.Instance != null)
        {
            GameManagerJasper.Instance.AddPoints(-200, false);
        }
        
        // Reset animation state
        if (playerAnimator != null)
        {
            playerAnimator.ResetToIdle();
        }
        
        // Give temporary invincibility
        invincibilityTimer = invincibilityDuration;
    }

    public bool IsCarryingItem()
    {
        return carriedItem != null;
    }

    public GameObject GetCarriedItem()
    {
        return carriedItem;
    }

    public void TakeDamage(float amount, Vector2 knockback)
    {
        if (isDead || invincibilityTimer > 0) return;

        currentHealth -= amount;
        invincibilityTimer = invincibilityDuration;
        
        // Apply knockback
        knockbackTimer = knockbackDuration;
        knockbackVelocity = knockback;

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (playerAnimator != null)
        {
            // TODO: Add hurt animation
            StartCoroutine(FlashSprite());
        }
    }

    private IEnumerator FlashSprite()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) yield break;

        Color normalColor = spriteRenderer.color;
        Color flashColor = Color.red;
        
        float flashDuration = 0.1f;
        int flashCount = 3;
        
        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = normalColor;
            yield return new WaitForSeconds(flashDuration);
        }
    }

    public void changeSpeed(float speed) {
        moveSpeed = speed;
    }
    public float getSpeed() {
        return moveSpeed;
    }
} 