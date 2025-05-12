using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ObjectivesPopupManager : MonoBehaviour
{
    public GameObject objectivesPopup;
    private GameManagerJasper gameManager;
    [SerializeField] private GameObject[] starImages;
    [SerializeField] private TextMeshProUGUI starText;

    void Start() {
        gameManager = GameManagerJasper.Instance;
    }

    void Update()
    {
        // Toggle popup when player presses 'O'
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (objectivesPopup != null)
                objectivesPopup.SetActive(!objectivesPopup.activeSelf);
                if (starText != null) {
                    starText.text = gameManager.formatStarText();
                }
        }
    }
} 