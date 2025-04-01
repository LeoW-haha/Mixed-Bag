using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int score = 0;       
    public Text scoreText;     

    void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore()
    {
        score++;
        
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }

    }
}
