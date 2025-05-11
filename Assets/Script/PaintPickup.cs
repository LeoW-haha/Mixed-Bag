using UnityEngine;

public class PaintPickup : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string paintColor; // Set this in the Inspector: "Red", "Green", "Black"

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerCtrl player = collision.GetComponent<PlayerCtrl>();
            if (player != null)
            {
                player.currentPackagingColour = paintColor;

                // Update paint UI display
                PaintDisplayUI paintUI = GameObject.FindObjectOfType<PaintDisplayUI>();
                if (paintUI != null)
                {
                    paintUI.UpdatePaintIcon(paintColor);
                }
            }
        }
    }
}

