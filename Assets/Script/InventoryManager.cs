using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject InventoryMenu;
    private bool menuActivated;
    public ItemSlot[] itemSlot;
    public ItemSO[] itemSOs;

    void Start()
    {
        InventoryMenu.SetActive(false);
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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && menuActivated) {
            Time.timeScale = 1;
            InventoryMenu.SetActive(false);
            menuActivated = false;
        } else if (Input.GetKeyDown(KeyCode.Tab) && !menuActivated) {
            Time.timeScale = 0;
            InventoryMenu.SetActive(true);
            menuActivated = true;
        } 
    }

    public void DeselectAllSlots() {
        for (int i = 0; i < itemSlot.Length; i++) {
            itemSlot[i].selectedShader.SetActive(false);
        }
    }
}
