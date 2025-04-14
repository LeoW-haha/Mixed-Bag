using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject InventoryMenu;
    public GameObject OrderMenu;
    public bool menuActivated;
    public bool orderMenuActivated;
    public ItemSlot[] itemSlot;
    public ItemSlot[] OrderItemSlot;
    public ItemSO[] itemSOs;
    private GameManager gameManager;

    void Start()
    {
        InventoryMenu.SetActive(false);
        OrderMenu.SetActive(false);
        menuActivated = false;
        
    }

    public bool useItem(string itemName) {
        for (int i = 0; i < itemSOs.Length; i++) {
            if(itemSOs[i].itemName == itemName) {
                return itemSOs[i].UseItem();
            }
        }
        return false;
    }

    public int getItemID(string itemName) {
        for (int i = 0; i < itemSOs.Length; i++) {
            if(itemSOs[i].itemName == itemName) {
                return itemSOs[i].getItemID();
            }
        }
        return 0;
    }

    public int[] getOrderItemIDs() {
        int[] OrderItemIDs = new int[OrderItemSlot.Length];
        for (int i = 0; i < OrderItemSlot.Length; i++) {
            OrderItemIDs[i] = this.getItemID(OrderItemSlot[i].itemName);
        }
        return OrderItemIDs;
    }

    public void clearOrderSlots() {
        for (int i = 0; i < OrderItemSlot.Length; i++) {
            OrderItemSlot[i].EmptySlot();
        }
    }

    public int addItem(string itemName, int quantity, Sprite itemSprite, string itemDescription) {
        for (int i = 0; i < itemSlot.Length; i++) {
            if((itemSlot[i].isFull == false && itemSlot[i].itemName == itemName) || itemSlot[i].quantity == 0) {
                int leftOverItems = itemSlot[i].addItem(itemName, quantity, itemSprite, itemDescription);

                if(leftOverItems > 0) {
                    leftOverItems = addItem(itemName, leftOverItems, itemSprite, itemDescription);
                }
                return leftOverItems;
            }
        }
        return quantity;
    }

    public int addOrderItem(string itemName, int quantity, Sprite itemSprite, string itemDescription) {
        for (int i = 0; i < OrderItemSlot.Length; i++) {
            if(OrderItemSlot[i].isFull == false && OrderItemSlot[i].itemName == itemName || OrderItemSlot[i].quantity == 0) {
                int leftOverItems = OrderItemSlot[i].addItem(itemName, quantity, itemSprite, itemDescription);
                
                if(leftOverItems > 0) {
                    leftOverItems = addOrderItem(itemName, leftOverItems, itemSprite, itemDescription);
                }
                return leftOverItems;
            }
        }
        return quantity;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Inventory") && menuActivated) {
            Time.timeScale = 1;
            InventoryMenu.SetActive(false);
            menuActivated = false;
            if(orderMenuActivated) {
                OrderMenu.SetActive(false);
                orderMenuActivated = false;                
            }
        } else if (Input.GetButtonDown("Inventory") && !menuActivated) {
            Time.timeScale = 0;
            InventoryMenu.SetActive(true);
            menuActivated = true;
        } 
    }

    public void DeselectAllSlots() {
        for (int i = 0; i < itemSlot.Length; i++) {
            itemSlot[i].selectedShader.SetActive(false);
        }
        for (int i = 0; i < OrderItemSlot.Length; i++) {
            OrderItemSlot[i].selectedShader.SetActive(false);
        }
    }
}
