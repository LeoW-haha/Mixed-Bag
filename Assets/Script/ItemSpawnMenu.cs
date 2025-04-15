using UnityEngine;

public class ItemSpawnMenu : MonoBehaviour
{
    private InventoryManager inventoryManager;
    private GameManager gameManager;
    public GameObject[] items;
    public ItemSlot[] itemSlot;
    public int startingQuantity;
    private Notifier notifier;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i<items.Length; i++) {
            Item currentItem = items[i].GetComponent<Item>();
            itemSlot[i].addItem(currentItem.itemName, startingQuantity, currentItem.sprite, currentItem.itemDescription, currentItem.restockCost);
        }
        notifier = GameObject.Find("NotificationHolder").GetComponent<Notifier>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public ItemSlot getSelectedSlot() {
        for (int i = 0; i<itemSlot.Length; i++) {
            if(itemSlot[i].thisItemSelected) {
                return itemSlot[i];
            }
        }        
        return null;
    }

    public void restockItem() {
        ItemSlot selectedItemSlot = getSelectedSlot();
        if (selectedItemSlot != null && gameManager.score >= selectedItemSlot.restockCost) {
            selectedItemSlot.addItem(selectedItemSlot.itemName, 3, selectedItemSlot.itemSprite, selectedItemSlot.itemDescription, selectedItemSlot.restockCost);  
            gameManager.minusScore(selectedItemSlot.restockCost);
        } else {
            notifier.Notify("Invalid order");
        }    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
