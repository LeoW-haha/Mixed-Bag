using UnityEngine;

public class PaintPickup : MonoBehaviour
{
    private string paintColor;

    private void Start()
    {
        paintColor = gameObject.name.Replace("(Clone)", "").Trim(); // Automatically use GameObject name
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerCtrl player = collision.GetComponent<PlayerCtrl>();
            if (player != null)
            {
                player.currentPackagingColour = paintColor;

                PaintDisplayUI paintUI = FindObjectOfType<PaintDisplayUI>();
                if (paintUI != null)
                {
                    paintUI.UpdatePaintIcon(paintColor);
                }
                else
                {
                    Debug.LogWarning("PaintDisplayUI not found in scene.");
                }

                Destroy(gameObject); // Optional
            }
        }
    }
}
