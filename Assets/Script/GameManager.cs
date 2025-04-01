using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int score = 0;       
    public Text scoreText;     

    void Start()
    {
        updateScore();
    }

    public void AddScore(){
        score++;

        updateScore();
    }

    void updateScore(){
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }

    }
}
