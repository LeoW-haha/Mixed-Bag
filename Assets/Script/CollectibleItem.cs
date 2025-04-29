using UnityEngine;
using System.Collections;

public class CollectibleItem : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private string itemName = "Item"; // Used to set the GameObject name
    [SerializeField] private int pointValue = 100; // Reserved for future scoring system
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip dropSound;
    [SerializeField] private float carryHeight = 1.5f; // Height above player when carried
    [SerializeField] private float startDelay = 0f; // Delay before item starts moving
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private bool canBePickedUp = true;
    [SerializeField] private float pickupRadius = 0.3f; // Reduced from 1f to 0.3f
    
    private bool isPickedUp = false;
    private Transform playerTransform;
    private ItemCarousel originalCarousel;
    private Vector3 originalScale;
    private SpriteRenderer spriteRenderer;
    private int originalSortOrder;
    private bool hasStarted = false;
    private CircleCollider2D pickupCollider;
    private bool isInRange = false;
    private GameObject player;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        
        originalSortOrder = spriteRenderer.sortingOrder;
        originalScale = transform.localScale;
        originalCarousel = GetComponentInParent<ItemCarousel>();
        
        // Use itemName to set the GameObject name
        gameObject.name = $"{itemName}_{pointValue}";
        
        // Set up trigger collider for pickup with smaller radius
        pickupCollider = GetComponent<CircleCollider2D>();
        if (pickupCollider == null)
        {
            pickupCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        pickupCollider.radius = pickupRadius;
        pickupCollider.isTrigger = true;
        
        // Set sprite if provided
        if (itemSprite != null)
        {
            spriteRenderer.sprite = itemSprite;
        }
        
        // Start the delay coroutine
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(startDelay);
        hasStarted = true;
    }

    private void Update()
    {
        if (!hasStarted) return; // Skip update until delay is finished
        
        if (isPickedUp && playerTransform != null)
        {
            // Follow the player when picked up
            Vector3 targetPos = playerTransform.position + new Vector3(0, carryHeight, 0);
            transform.position = targetPos;
        }

        if (isInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryPickup();
        }
    }

    public void PickUp(Transform player)
    {
        if (!isPickedUp)
        {
            isPickedUp = true;
            playerTransform = player;
            
            // Detach from carousel and any parent
            transform.parent = null;
            
            // Play sound
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            // Adjust sprite order to be above player
            spriteRenderer.sortingOrder = originalSortOrder + 2;
            
            // Enable collider
            if (pickupCollider != null)
            {
                pickupCollider.enabled = false;
            }
        }
    }

    public void Drop()
    {
        if (isPickedUp)
        {
            isPickedUp = false;
            playerTransform = null;

            // Play sound
            if (dropSound != null)
            {
                AudioSource.PlayClipAtPoint(dropSound, transform.position);
            }

            // Reset sprite order
            spriteRenderer.sortingOrder = originalSortOrder;
            
            // Re-enable collider
            if (pickupCollider != null)
            {
                pickupCollider.enabled = true;
            }
        }
    }

    public bool IsPickedUp()
    {
        return isPickedUp;
    }

    // Getter methods for the private fields
    public string GetItemName() { return itemName; }
    public int GetPointValue() { return pointValue; }
    public Sprite GetItemSprite() { return itemSprite; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canBePickedUp || isPickedUp) return;
        
        if (other.CompareTag("Player"))
        {
            isInRange = true;
            player = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
            player = null;
        }
    }

    private void TryPickup()
    {
        if (!canBePickedUp || player == null || isPickedUp) return;

        var playerController = player.GetComponent<PlayerController>();
        if (playerController != null && playerController.CanPickupItem())
        {
            playerController.PickupItem(gameObject);
        }
    }
} 