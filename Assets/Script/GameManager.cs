using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private GameObject[] starImages;
    
    [Header("Level Settings")]
    [SerializeField] private float levelTimeLimit = 180f; // 3 minutes default
    [SerializeField] private int scoreForOneStar = 1000;
    [SerializeField] private int scoreForTwoStars = 2000;
    [SerializeField] private int scoreForThreeStars = 3000;
    [SerializeField] private float feedbackDisplayTime = 2f;
    
    [Header("Score Settings")]
    [SerializeField] private int comboBonus = 50; // Additional points for quick successive deliveries
    [SerializeField] private float comboTimeWindow = 5f; // Time window for combo bonus
    
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<GameManager>();
            }
            return instance;
        }
    }
    
    private int currentScore = 0;
    private float levelTimer = 0f;
    private float lastDeliveryTime = 0f;
    private int currentCombo = 0;
    private bool isLevelActive = false;
    private int currentStars = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ResetLevel();
        StartLevel();
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isLevelActive)
        {
            UpdateTimer();
            CheckStarRating();
        }
    }

    private void UpdateTimer()
    {
        if (levelTimer > 0)
        {
            levelTimer -= Time.deltaTime;
            UpdateTimerDisplay();

            if (levelTimer <= 0)
            {
                levelTimer = 0;
                EndLevel();
            }
        }
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(levelTimer / 60);
            int seconds = Mathf.FloorToInt(levelTimer % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void AddPoints(int points, string itemName = "")
    {
        if (!isLevelActive) return;

        // Check for combo
        float timeSinceLastDelivery = Time.time - lastDeliveryTime;
        if (timeSinceLastDelivery < comboTimeWindow)
        {
            currentCombo++;
            points += comboBonus * currentCombo;
        }
        else
        {
            currentCombo = 0;
        }
        
        lastDeliveryTime = Time.time;
        currentScore += points;
        UpdateScoreDisplay();
        
        // Show feedback
        string feedbackMessage = $"+{points} points";
        if (currentCombo > 0)
        {
            feedbackMessage += $"\nCombo x{currentCombo + 1}!";
        }
        if (!string.IsNullOrEmpty(itemName))
        {
            feedbackMessage = $"{itemName}\n{feedbackMessage}";
        }
        ShowFeedback(feedbackMessage);

        // Check if star rating changed
        CheckStarRating();
    }

    private void CheckStarRating()
    {
        int newStars = 0;
        
        if (currentScore >= scoreForThreeStars)
            newStars = 3;
        else if (currentScore >= scoreForTwoStars)
            newStars = 2;
        else if (currentScore >= scoreForOneStar)
            newStars = 1;

        if (newStars != currentStars)
        {
            currentStars = newStars;
            UpdateStarDisplay();
            if (newStars > 0)
            {
                ShowFeedback($"Achieved {newStars} Star Rating!");
            }
        }
    }

    private void UpdateStarDisplay()
    {
        if (starImages == null) return;
        
        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] != null)
            {
                starImages[i].SetActive(i < currentStars);
            }
        }
    }

    public void ShowFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(true);
            feedbackText.text = message;
            CancelInvoke(nameof(HideFeedback));
            Invoke(nameof(HideFeedback), feedbackDisplayTime);
        }
    }

    private void HideFeedback()
    {
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(false);
        }
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
    }

    public void StartLevel()
    {
        isLevelActive = true;
        levelTimer = levelTimeLimit;
        UpdateTimerDisplay();
    }

    public void EndLevel()
    {
        isLevelActive = false;
        ShowFeedback($"Level Complete!\nFinal Score: {currentScore}\nStars Earned: {currentStars}");
    }

    public void ResetLevel()
    {
        currentScore = 0;
        currentCombo = 0;
        currentStars = 0;
        levelTimer = levelTimeLimit;
        isLevelActive = false;
        UpdateScoreDisplay();
        UpdateTimerDisplay();
        UpdateStarDisplay();
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public int GetCurrentStars()
    {
        return currentStars;
    }
} 