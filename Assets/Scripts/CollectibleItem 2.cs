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
    
    private bool isPickedUp = false;
    private Transform playerTransform;
    private ItemCarousel originalCarousel;
    private Vector3 originalScale;
    private SpriteRenderer spriteRenderer;
    private int originalSortOrder;
    private bool hasStarted = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSortOrder = spriteRenderer.sortingOrder;
        originalScale = transform.localScale;
        originalCarousel = GetComponentInParent<ItemCarousel>();
        
        // Use itemName to set the GameObject name
        gameObject.name = $"{itemName}_{pointValue}";
        
        // Start the delay coroutine
        StartCoroutine(DelayedStart());
    }

    private System.Collections.IEnumerator DelayedStart()
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
        }
    }

    public bool IsPickedUp()
    {
        return isPickedUp;
    }

    // Getter methods for the private fields
    public string GetItemName() { return itemName; }
    public int GetPointValue() { return pointValue; }
} 