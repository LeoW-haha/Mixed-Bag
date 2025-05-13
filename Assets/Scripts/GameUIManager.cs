using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    public Image orderImage;

    private GameManagerJasper gameManager;
    private OrderSystem orderSystem;
    private Coroutine gameTimerFlashCoroutine;
    private bool isGameTimerFlashing = false;

    [Header("Fuel Info")]
    public Image equippedFuelImage;
    public Sprite redFuelSprite;
    public Sprite greenFuelSprite;
    public Sprite blackFuelSprite;
    public Sprite whiteFuelSprite;



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
            {
                int score = gameManager.GetCurrentScore();
                scoreText.text = $"Score: {score}";
                if (score < 0)
                    scoreText.color = Color.red;
                else if (score > 0)
                    scoreText.color = new Color(1f, 0.84f, 0f); // Gold
                else
                    scoreText.color = Color.white;
            }
            if (gameTimerText != null)
            {
                float timeLeft = Mathf.Max(0, gameManager.levelTimeLimit - Time.timeSinceLevelLoad);
                int minutes = Mathf.FloorToInt(timeLeft / 60);
                int seconds = Mathf.FloorToInt(timeLeft % 60);
                gameTimerText.text = $"Time: {minutes:00}:{seconds:00}";

                if (timeLeft <= 60)
                {
                    if (!isGameTimerFlashing)
                    {
                        isGameTimerFlashing = true;
                        if (gameTimerFlashCoroutine != null)
                            StopCoroutine(gameTimerFlashCoroutine);
                        gameTimerFlashCoroutine = StartCoroutine(FlashGameTimer());
                    }
                }
                else
                {
                    if (isGameTimerFlashing)
                    {
                        isGameTimerFlashing = false;
                        if (gameTimerFlashCoroutine != null)
                            StopCoroutine(gameTimerFlashCoroutine);
                        gameTimerText.color = Color.white;
                    }
                    else
                    {
                        gameTimerText.color = Color.white;
                    }
                }
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
                {
                    int totalSeconds = Mathf.CeilToInt(orderSystem.GetCurrentTime());
                    int minutes = totalSeconds / 60;
                    int seconds = totalSeconds % 60;
                    orderTimerText.text = $"{minutes:00}:{seconds:00}";
                    orderTimerText.color = totalSeconds <= 10 ? Color.red : Color.white;
                }
                if (orderImage != null)
                    orderImage.sprite = currentOrder.itemSprite;
            }
            else
            {
                if (orderText != null) orderText.text = "Order: None";
                if (orderPointsText != null) orderPointsText.text = "";
                if (orderTimerText != null) orderTimerText.text = "";
                if (orderImage != null) orderImage.sprite = null;
            }
        }
    }

public void SetEquippedFuel(string fuelTag)
    {
        if (equippedFuelImage == null) return; // Skip if fuel UI not set

        Sprite fuelSprite = GetFuelSpriteByTag(fuelTag);
        if (fuelSprite != null)
            equippedFuelImage.sprite = fuelSprite;
    }


    private Sprite GetFuelSpriteByTag(string tag)
    {
        // You must assign these in the Inspector or load via Resources
        switch (tag)
        {
            case "Red": return redFuelSprite;
            case "Green": return greenFuelSprite;
            case "Black": return blackFuelSprite;
            case "White": return whiteFuelSprite;
            default: return null;
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

    private System.Collections.IEnumerator FlashGameTimer()
    {
        Color normalColor = Color.red;
        Color flashColor = Color.white;
        float flashInterval = 0.5f;

        while (true)
        {
            if (gameTimerText != null)
                gameTimerText.color = flashColor;
            yield return new WaitForSeconds(flashInterval);
            if (gameTimerText != null)
                gameTimerText.color = normalColor;
            yield return new WaitForSeconds(flashInterval);
        }
    }
} 