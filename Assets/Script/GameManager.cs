using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public int score = 0;         
    public Text scoreText;
    [SerializeField] private Image[] orderIcons;
    [SerializeField] private GameObject[] Items;
    private GameObject[] orderItems;
    public int maxOrderAmount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        orderItems = new GameObject[maxOrderAmount];
        UpdateScoreUI();
        for (int i = 0; i < orderIcons.Length; i++) {
            orderItems[i] = Items[Random.Range(0, Items.Length-1)];
            orderIcons[i].sprite = orderItems[i].GetComponent<Item>().sprite;
        }
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
