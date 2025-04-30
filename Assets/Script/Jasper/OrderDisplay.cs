using UnityEngine;
using TMPro;

public class OrderDisplay : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private OrderSystem orderSystem;
    private GameManagerJasper gameManager;
    
    [Header("UI References")]
    [SerializeField] private TextMeshPro orderText;
    [SerializeField] private TextMeshPro timerText;
    [SerializeField] private TextMeshPro scoreText;
    
    [Header("Display Settings")]
    [SerializeField] private float displayScale = 1f;
    [SerializeField] private Color timerNormalColor = Color.white;
    [SerializeField] private Color timerWarningColor = Color.yellow;
    [SerializeField] private Color timerCriticalColor = Color.red;
    [SerializeField] private float warningThreshold = 10f;
    [SerializeField] private float criticalThreshold = 5f;
    
    [Header("Text Positioning")]
    [SerializeField] private float startingOffset = -1.5f; // Where the first text element starts
    [SerializeField] private float spacingBetweenTexts = 0.8f; // Space between each text element
    
    [Header("Rendering Settings")]
    [SerializeField] private string sortingLayerName = "UI";
    [SerializeField] private int orderInLayer = 10;

    private void Start()
    {
        // Get or add SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        
        // Set sprite rendering order
        spriteRenderer.sortingLayerName = sortingLayerName;
        spriteRenderer.sortingOrder = orderInLayer;

        // Create TextMeshPro components if not assigned
        if (timerText == null)
        {
            GameObject timerTextObj = new GameObject("TimerText");
            timerTextObj.transform.SetParent(transform);
            timerText = timerTextObj.AddComponent<TextMeshPro>();
            timerText.alignment = TextAlignmentOptions.Center;
            timerText.fontSize = 3;
        }
        
        // Position timer text (first element)
        timerText.transform.localPosition = new Vector3(0, startingOffset, 0);
        
        // Set timer text rendering order
        timerText.sortingLayerID = SortingLayer.NameToID(sortingLayerName);
        timerText.sortingOrder = orderInLayer + 1;

        if (orderText == null)
        {
            GameObject orderTextObj = new GameObject("OrderText");
            orderTextObj.transform.SetParent(transform);
            orderText = orderTextObj.AddComponent<TextMeshPro>();
            orderText.alignment = TextAlignmentOptions.Center;
            orderText.fontSize = 3;
        }
        
        // Position order text (second element)
        orderText.transform.localPosition = new Vector3(0, startingOffset - spacingBetweenTexts, 0);
        
        // Set order text rendering order
        orderText.sortingLayerID = SortingLayer.NameToID(sortingLayerName);
        orderText.sortingOrder = orderInLayer + 1;

        // Create and position score text (third element)
        if (scoreText == null)
        {
            GameObject scoreTextObj = new GameObject("ScoreText");
            scoreTextObj.transform.SetParent(transform);
            scoreText = scoreTextObj.AddComponent<TextMeshPro>();
            scoreText.alignment = TextAlignmentOptions.Center;
            scoreText.fontSize = 3;
        }
        
        // Position score text below order text
        scoreText.transform.localPosition = new Vector3(0, startingOffset - (spacingBetweenTexts * 2), 0);
        
        // Set score text rendering order
        scoreText.sortingLayerID = SortingLayer.NameToID(sortingLayerName);
        scoreText.sortingOrder = orderInLayer + 1;

        // Set the scale
        transform.localScale = Vector3.one * displayScale;

        // Find required systems
        orderSystem = FindFirstObjectByType<OrderSystem>();
        gameManager = GameManagerJasper.Instance;
        
        if (orderSystem == null)
        {
            Debug.LogError("OrderDisplay: OrderSystem not found! Create an OrderSystem first.");
            enabled = false;
            return;
        }
        
        if (gameManager == null)
        {
            Debug.LogError("OrderDisplay: GameManager not found!");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (orderSystem == null || spriteRenderer == null) return;

        OrderSystem.OrderItem currentOrder = orderSystem.GetCurrentOrder();
        if (currentOrder != null)
        {
            // Update sprite
            spriteRenderer.sprite = currentOrder.itemSprite;
            spriteRenderer.enabled = true;

            // Update timer text (first element)
            if (timerText != null)
            {
                float timeLeft = orderSystem.GetCurrentTime();
                timerText.text = $"Time: {Mathf.CeilToInt(timeLeft)}s";
                
                // Update timer color based on time remaining
                if (timeLeft <= criticalThreshold)
                    timerText.color = timerCriticalColor;
                else if (timeLeft <= warningThreshold)
                    timerText.color = timerWarningColor;
                else
                    timerText.color = timerNormalColor;
                
                timerText.enabled = true;
            }

            // Update order text (second element)
            if (orderText != null)
            {
                orderText.text = $"Need: {currentOrder.itemName}\nReward: {currentOrder.pointsReward}";
                orderText.enabled = true;
            }

            // Update score text (third element)
            if (scoreText != null && gameManager != null)
            {
                scoreText.text = $"Score: {gameManager.GetCurrentScore()}";
                scoreText.enabled = true;
            }
        }
        else
        {
            // Hide everything if no order
            spriteRenderer.enabled = false;
            if (orderText != null) orderText.enabled = false;
            if (timerText != null) timerText.enabled = false;
            if (scoreText != null) scoreText.enabled = false;
        }
    }
} 