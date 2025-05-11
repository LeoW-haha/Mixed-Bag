using UnityEngine;

public class ControlsPopupManager : MonoBehaviour
{
    public GameObject controlsPopup;

    void Update()
    {
        // Toggle popup when player presses 'H'
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (controlsPopup != null)
                controlsPopup.SetActive(!controlsPopup.activeSelf);
        }
    }
} 