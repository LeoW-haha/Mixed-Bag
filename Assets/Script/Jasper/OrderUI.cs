using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OrderUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI orderText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI pointsText;

    private void Start()
    {
        // Verify components are assigned
        if (orderText == null || timerText == null || pointsText == null)
        {
            Debug.LogError("OrderUI: Missing UI text components!");
        }
    }

    // Called by OrderSystem.OnNewOrder event
    public void OnNewOrder(OrderSystem.OrderItem order)
    {
        if (orderText != null)
        {
            orderText.text = $"Deliver: {order.itemName}\nReward: {order.pointsReward}";
        }
    }

    // Called by OrderSystem.OnTimeChanged event
    public void OnTimeChanged(float timeRemaining)
    {
        if (timerText != null)
        {
            timerText.text = $"Time: {Mathf.CeilToInt(timeRemaining)}s";
        }
    }

    // Called by OrderSystem.OnPointsChanged event
    public void OnPointsChanged(int points)
    {
        if (pointsText != null)
        {
            pointsText.text = $"Points: {points}";
        }
    }
} 