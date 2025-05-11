using UnityEngine;
using UnityEngine.UI;

public class PaintDisplayUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Image paintIcon;
    public Sprite redSprite;
    public Sprite greenSprite;
    public Sprite blackSprite;

    public void UpdatePaintIcon(string color)
    {
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
        }
    }
}
