using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class DeliveryPoint : MonoBehaviour
{
    [SerializeField] private float interactionRadius = 1f;
    [SerializeField] private int pointsPerItem = 100;
    [SerializeField] private TextMeshProUGUI scoreText;
    
    private int totalScore = 0;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }

    public bool IsPlayerInRange(Vector2 playerPosition)
    {
        return Vector2.Distance(playerPosition, transform.position) <= interactionRadius;
    }

    public void DeliverItems(List<GameObject> items)
    {
        foreach (GameObject item in items)
        {
            // Add points for each item delivered
            totalScore += pointsPerItem;
            
            // Destroy the item
            Destroy(item);
        }

        // Update the score display
        if (scoreText != null)
        {
            scoreText.text = $"Score: {totalScore}";
        }
    }
} 