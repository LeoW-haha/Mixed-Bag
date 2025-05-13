using UnityEngine;
using UnityEngine.UI;

public class PaintDisplayUI : MonoBehaviour
{
    public Image paintIcon;
    public Sprite redSprite;
    public Sprite greenSprite;
    public Sprite blackSprite;

    public void UpdatePaintIcon(string color)
    {
        if (paintIcon == null)
        {
            Debug.LogWarning("Paint icon UI is not assigned in the inspector.");
            return;
        }

        switch (color)
        {
            case "Red":
                paintIcon.sprite = redSprite;
                break;
            case "Green":
                paintIcon.sprite = greenSprite;
                break;
            case "Black":
                paintIcon.sprite = blackSprite;
                break;
            default:
                Debug.LogWarning("Unknown paint color: " + color);
                break;
        }
    }
}