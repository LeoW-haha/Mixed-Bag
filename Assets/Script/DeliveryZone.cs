using UnityEngine;
using System.Collections;

public class DeliveryZone : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Color highlightColor = new Color(0.5f, 1f, 0.5f, 0.7f);
    [SerializeField] private Color wrongDeliveryColor = new Color(1f, 0.5f, 0.5f, 0.7f);
    
    [Header("Audio")]
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip failureSound;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isPlayerInRange = false;
    private GameObject playerInRange;
    private OrderSystem orderSystem;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        orderSystem = FindFirstObjectByType<OrderSystem>();
        if (orderSystem == null)
        {
            Debug.LogError("DeliveryZone: OrderSystem not found!");
            enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerInRange = other.gameObject;
            
            // Highlight the zone
            if (spriteRenderer != null)
            {
                spriteRenderer.color = highlightColor;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerInRange = null;
            
            // Reset color
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            TryDeliverItem();
        }
    }

    private void TryDeliverItem()
    {
        if (!isPlayerInRange || playerInRange == null || orderSystem == null) return;

        PlayerController playerController = playerInRange.GetComponent<PlayerController>();
        if (playerController == null || !playerController.IsCarryingItem()) return;

        GameObject carriedItem = playerController.GetCarriedItem();
        CollectibleItem item = carriedItem.GetComponent<CollectibleItem>();
        
        if (item == null) return;

        // Get current order from OrderSystem
        OrderSystem.OrderItem currentOrder = orderSystem.GetCurrentOrder();
        if (currentOrder == null) return;

        // Check if delivered item matches the order
        if (item.GetItemName() == currentOrder.itemName)
        {
            // Successful delivery
            PlaySound(successSound);
            orderSystem.DeliverItem(item.GetItemName());
            
            // Remove the item
            playerController.DropItem();
            Destroy(carriedItem);
            
            // Show success highlight briefly
            StartCoroutine(FlashColor(highlightColor));
        }
        else
        {
            // Wrong item delivered
            PlaySound(failureSound);
            orderSystem.DeliverItem(item.GetItemName()); // This will trigger the penalty
            
            // Show failure highlight briefly
            StartCoroutine(FlashColor(wrongDeliveryColor));
        }
    }

    private IEnumerator FlashColor(Color flashColor)
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(0.5f);
            spriteRenderer.color = originalColor;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }
    }
} 