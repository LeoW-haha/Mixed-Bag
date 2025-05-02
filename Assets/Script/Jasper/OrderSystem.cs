using UnityEngine;
using System.Collections.Generic;
using System;

public class OrderSystem : MonoBehaviour
{
    [System.Serializable]
    public class OrderItem
    {
        public string itemName;
        public int pointsReward;
        public float timeLimit;
        public Sprite itemSprite;
    }

    [Header("Order Settings")]
    [SerializeField] private List<OrderItem> possibleOrders;
    [SerializeField] private float defaultTimeLimit = 30f;
    [SerializeField] private int wrongDeliveryPenalty = 100;

    private OrderItem currentOrder;
    private float currentTime;
    private bool isOrderActive;
    private GameManagerJasper gameManager;

    private void Start()
    {
        Debug.Log("OrderSystem: Starting...");
        
        gameManager = GameManagerJasper.Instance;
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found!");
            enabled = false;
            return;
        }

        if (possibleOrders == null || possibleOrders.Count == 0)
        {
            Debug.LogError("OrderSystem: No orders configured!");
            enabled = false;
            return;
        }

        Debug.Log($"OrderSystem: Found {possibleOrders.Count} possible orders");
        GenerateNewOrder();
        Debug.Log("OrderSystem: Start complete");
    }

    private void Update()
    {
        if (!isOrderActive || gameManager == null) return;

        currentTime -= Time.deltaTime;
        
        if (currentTime <= 0)
        {
            OrderFailed();
        }
    }

    public void GenerateNewOrder()
    {
        Debug.Log("OrderSystem: Generating new order...");
        
        if (possibleOrders == null || possibleOrders.Count == 0)
        {
            Debug.LogError("OrderSystem: No orders configured!");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, possibleOrders.Count);
        currentOrder = possibleOrders[randomIndex];
        
        if (currentOrder == null)
        {
            Debug.LogError("OrderSystem: Selected order is null!");
            return;
        }

        Debug.Log($"OrderSystem: New order generated - {currentOrder.itemName}");
        
        currentTime = currentOrder.timeLimit > 0 ? currentOrder.timeLimit : defaultTimeLimit;
        isOrderActive = true;

        if (gameManager != null)
        {
            gameManager.ShowFeedback($"New Order: {currentOrder.itemName}");
        }
    }

    public void DeliverItem(string itemName)
    {
        if (!isOrderActive || gameManager == null) return;

        Debug.Log($"OrderSystem: Attempting to deliver item: {itemName}");

        if (itemName == currentOrder.itemName)
        {
            // Correct delivery
            gameManager.AddPoints(currentOrder.pointsReward, true, currentOrder.itemName);
            Debug.Log($"OrderSystem: Correct delivery!");
            
            // Generate new order
            GenerateNewOrder();
        }
        else
        {
            // Wrong delivery
            gameManager.AddPoints(-wrongDeliveryPenalty, false, "Wrong Item!");
            Debug.Log($"OrderSystem: Wrong delivery.");
        }
    }

    private void OrderFailed()
    {
        Debug.Log("OrderSystem: Order failed due to timeout");
        isOrderActive = false;
        
        if (gameManager != null)
        {
            gameManager.AddPoints(-wrongDeliveryPenalty, false, "Time's up!");
        }
        
        // Generate new order after a short delay
        Invoke(nameof(GenerateNewOrder), 2f);
    }

    public OrderItem GetCurrentOrder()
    {
        return currentOrder;
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }
} 