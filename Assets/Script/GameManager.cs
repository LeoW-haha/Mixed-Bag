using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public int score = 0;         
    public Text scoreText;
    public GameObject[] itemSlots;
    public Sprite[] itemSprites;
    public float changeInterval = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateScoreUI();
        InvokeRepeating(nameof(SetRandomCombination), 0f, changeInterval);
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
    void SetRandomCombination()
    {
        foreach (GameObject slot in itemSlots)
        {

            Image img = slot.GetComponentInChildren<Image>();
            if (img != null && itemSprites.Length > 0)
            {
                img.sprite = itemSprites[Random.Range(0, itemSprites.Length)];
            }

        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
