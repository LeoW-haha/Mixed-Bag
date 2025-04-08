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

    void Start()
    {
        InventoryMenu.SetActive(false);
        OrderMenu.SetActive(false);
        menuActivated = false;
    }

    public int getItemID(string itemName) {
        for (int i = 0; i < itemSOs.Length; i++) {
            if(itemSOs[i].itemName == itemName) {
                return itemSOs[i].id;
            }
        }
        return 0;
    }

    public int addItem(string itemName, int quantity, Sprite itemSprite, string itemDescription) {
        for (int i = 0; i < itemSlot.Length; i++) {
            if(itemSlot[i].isFull == false && itemSlot[i].itemName == itemName || itemSlot[i].quantity == 0) {
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
