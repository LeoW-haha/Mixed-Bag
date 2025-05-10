using UnityEngine;

public class ObjectivesPopupManager : MonoBehaviour
{
    public GameObject objectivesPopup;

    void Update()
    {
        // Toggle popup when player presses 'O'
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (objectivesPopup != null)
                objectivesPopup.SetActive(!objectivesPopup.activeSelf);
        }
    }
} 