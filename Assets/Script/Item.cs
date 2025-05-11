using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] public string itemName;
    [SerializeField] public int quantity;
    [SerializeField] public Sprite sprite;
    public int startAmount;

    private InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas")?.GetComponent<InventoryManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerCtrl player = collision.gameObject.GetComponent<PlayerCtrl>();
            if (player != null)
            {
                if (player.hasPackage)
                {
                    Debug.Log("❌ Cannot pick up item while holding a package.");
                    return;
                }

                if (inventoryManager != null)
                {
                    int leftOverItems = inventoryManager.addItem(itemName, quantity, sprite);

                    if (leftOverItems <= 0)
                    {
                        Destroy(gameObject);
                    }
                    else
                    {
                        quantity = leftOverItems;
                    }
                }
                else
                {
                    Debug.LogWarning("InventoryManager not found.");
                }
            }
        }
    }
}
