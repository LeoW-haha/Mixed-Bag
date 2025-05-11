using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialManagerWater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI feedbackText;
    public bool tutorialFinished;
    [SerializeField] private WaterPipeJasper waterPipe;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        feedbackText.text = "Fix the leak with the leak fixer [Space]";
        waterPipe.activateWaterPipe();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!waterPipe.complete) {
            feedbackText.text = "Fix the leak with the leak fixer [Space]";
        }

    }
}
