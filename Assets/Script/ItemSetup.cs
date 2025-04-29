using UnityEngine;

public class ItemSetup : MonoBehaviour
{
    private void Awake()
    {
        // Make sure the item has the required components
        if (!GetComponent<BoxCollider2D>())
        {
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
        }

        // Set the tag
        if (gameObject.tag != "Item")
        {
            gameObject.tag = "Item";
        }
    }

    private void OnValidate()
    {
        // This ensures proper setup in the editor
        if (!GetComponent<BoxCollider2D>())
        {
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
        }

        if (gameObject.tag != "Item")
        {
            gameObject.tag = "Item";
        }
    }
} 