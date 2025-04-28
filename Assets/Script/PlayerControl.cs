using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float interactionRadius = 1f;

    private Rigidbody2D rb;
    private float speedX;
    private float speedY;
    private PlayerAnimator animator;
    private CollectibleItem carriedItem;
    private bool isCarrying = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<PlayerAnimator>();
    }

    void Update()
    {
        // Get input
        speedX = Input.GetAxisRaw("Horizontal");
        speedY = Input.GetAxisRaw("Vertical");

        // Handle item interaction with E key
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isCarrying)
            {
                TryPickUpItem();
            }
            else
            {
                DropItem();
            }
        }
    }

    void FixedUpdate()
    {
        // Apply movement
        rb.linearVelocity = new Vector2(speedX * moveSpeed, speedY * moveSpeed);
    }

    private void TryPickUpItem()
    {
        // Check for nearby items
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRadius);
        bool foundItem = false;
        
        foreach (Collider2D collider in colliders)
        {
            CollectibleItem item = collider.GetComponent<CollectibleItem>();
            if (item != null && !item.IsPickedUp())
            {
                // Pick up the item
                carriedItem = item;
                item.PickUp(transform);
                isCarrying = true;
                foundItem = true;
                
                // Tell animator to play carrying animation
                if (animator != null)
                {
                    animator.SetCarrying(true);
                }
                
                break;
            }
        }

        // Only update carrying state if we actually found an item
        if (!foundItem)
        {
            isCarrying = false;
            if (animator != null)
            {
                animator.SetCarrying(false);
            }
        }
    }

    private void DropItem()
    {
        if (carriedItem != null)
        {
            carriedItem.Drop();
            carriedItem = null;
            isCarrying = false;
            
            // Tell animator to stop carrying animation
            if (animator != null)
            {
                animator.SetCarrying(false);
            }
        }
    }

    // Optional: Visualize interaction radius in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
