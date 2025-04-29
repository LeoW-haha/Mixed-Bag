using UnityEngine;
using TMPro;

public class SimplePickupSystem : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform carryPoint;
    [SerializeField] private float pickupRadius = 1f;
    [SerializeField] private TextMeshProUGUI scoreText;
    
    private Rigidbody2D rb;
    private GameObject carriedItem;
    private int score = 0;
    private bool isCarrying = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (scoreText != null)
        {
            UpdateScoreText();
        }
    }

    private void Update()
    {
        // Movement
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector2(moveX, moveY).normalized;
        rb.linearVelocity = movement * moveSpeed;

        // Pickup/Drop with R key
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isCarrying)
            {
                TryPickupItem();
            }
            else
            {
                DropItem();
            }
        }

        // Deliver with T key
        if (Input.GetKeyDown(KeyCode.T) && isCarrying)
        {
            DeliverItem();
        }
    }

    private void TryPickupItem()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pickupRadius);
        
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Item"))
            {
                carriedItem = collider.gameObject;
                carriedItem.transform.SetParent(carryPoint);
                carriedItem.transform.localPosition = Vector3.zero;
                isCarrying = true;
                break;
            }
        }
    }

    private void DropItem()
    {
        if (carriedItem != null)
        {
            carriedItem.transform.SetParent(null);
            carriedItem = null;
            isCarrying = false;
        }
    }

    private void DeliverItem()
    {
        if (carriedItem != null)
        {
            score += 100;
            UpdateScoreText();
            Destroy(carriedItem);
            carriedItem = null;
            isCarrying = false;
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
} 