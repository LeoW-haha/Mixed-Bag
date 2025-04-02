using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public int score = 0;         
    public Text scoreText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateScoreUI();
    }
    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();
    }
    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

        // Update is called once per frame
        void Update()
    {
        
    }
}
