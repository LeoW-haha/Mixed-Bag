using UnityEngine;

public class Item : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    public string itemName;
    [SerializeField]
    public int quantity;
    [SerializeField]
    public Sprite sprite;
    public float restockCost;
    [TextArea]
    [SerializeField]
    public string itemDescription;
    public int startAmount;

    private InventoryManager inventoryManager;
    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (collision.gameObject.tag == "Player") {
                int leftOverItems = inventoryManager.addItem(itemName, quantity, sprite, itemDescription, restockCost);
                if (leftOverItems <=0 ) {
                    Destroy(gameObject);
            } else {
                quantity = leftOverItems;
            }
        }
    }
}
