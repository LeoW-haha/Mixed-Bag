using UnityEngine;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    [Header("Left Panel")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameTimerText;
    public TextMeshProUGUI feedbackText;

    [Header("Right Panel")]
    public TextMeshProUGUI orderText;
    public TextMeshProUGUI orderPointsText;
    public TextMeshProUGUI orderTimerText;

    private GameManagerJasper gameManager;
    private OrderSystem orderSystem;

    void Start()
    {
        gameManager = GameManagerJasper.Instance;
        orderSystem = FindFirstObjectByType<OrderSystem>();
        if (feedbackText != null) feedbackText.text = "";
    }

    void Update()
    {
        // Update score and game timer
        if (gameManager != null)
        {
            if (scoreText != null)
                scoreText.text = $"Score: {gameManager.GetCurrentScore()}";
            if (gameTimerText != null)
            {
                float timeLeft = Mathf.Max(0, gameManager.levelTimeLimit - Time.timeSinceLevelLoad);
                int minutes = Mathf.FloorToInt(timeLeft / 60);
                int seconds = Mathf.FloorToInt(timeLeft % 60);
                gameTimerText.text = $"Time: {minutes:00}:{seconds:00}";
            }
        }

        // Update order info
        if (orderSystem != null)
        {
            var currentOrder = orderSystem.GetCurrentOrder();
            if (currentOrder != null)
            {
                if (orderText != null)
                    orderText.text = $"Order: {currentOrder.itemName}";
                if (orderPointsText != null)
                    orderPointsText.text = $"Points: {currentOrder.pointsReward}";
                if (orderTimerText != null)
                    orderTimerText.text = $"Order Time: {Mathf.CeilToInt(orderSystem.GetCurrentTime())}s";
            }
            else
            {
                if (orderText != null) orderText.text = "Order: None";
                if (orderPointsText != null) orderPointsText.text = "";
                if (orderTimerText != null) orderTimerText.text = "";
            }
        }
    }

    // Call this from your game logic to show feedback
    public void ShowFeedback(string message, float duration = 2f)
    {
        StopAllCoroutines();
        StartCoroutine(ShowFeedbackRoutine(message, duration));
    }

    private System.Collections.IEnumerator ShowFeedbackRoutine(string message, float duration)
    {
        if (feedbackText != null)
            feedbackText.text = message;
        yield return new WaitForSeconds(duration);
        if (feedbackText != null)
            feedbackText.text = "";
    }
} 