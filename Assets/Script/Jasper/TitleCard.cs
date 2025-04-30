using UnityEngine;
using TMPro;

public class TitleCard : MonoBehaviour
{
    [SerializeField] private GameObject titleCardPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI rulesText;
    [SerializeField] private TextMeshProUGUI pressSpaceText;
    
    private void Start()
    {
        // Make sure the panel is visible at start
        titleCardPanel.SetActive(true);
        
        // Pause the game
        Time.timeScale = 0f;
    }
    
    private void Update()
    {
        // Check for Space key press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }
    
    private void StartGame()
    {
        // Hide the title card
        titleCardPanel.SetActive(false);
        
        // Resume the game
        Time.timeScale = 1f;
    }
    
    public void SetTitle(string title)
    {
        if (titleText != null)
        {
            titleText.text = title;
        }
    }
    
    public void SetRules(string rules)
    {
        if (rulesText != null)
        {
            rulesText.text = rules;
        }
    }
} 