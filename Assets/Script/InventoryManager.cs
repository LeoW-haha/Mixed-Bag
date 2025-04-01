using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject InventoryMenu;
    private bool menuActivated;
    public ItemSlot[] itemSlot;

    void Start()
    {
        
    }

    public void addItem(string itemName, int quantity, Sprite itemSprite) {
        for (int i = 0; i < itemSlot.Length; i++) {
            if(itemSlot[i].isFull == false) {
                itemSlot[i].addItem(itemName, quantity, itemSprite);
                return;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Inventory") && menuActivated) {
            Time.timeScale = 1;
            InventoryMenu.SetActive(false);
            menuActivated = false;
        } else if (Input.GetButtonDown("Inventory") && !menuActivated) {
            Time.timeScale = 0;
            InventoryMenu.SetActive(true);
            menuActivated = true;
        } 
    }
}
