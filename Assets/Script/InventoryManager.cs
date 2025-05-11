using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public ItemSlot[] itemSlot;
    public ItemSlot[] OrderItemSlot;
    public ItemSlot[] SpawnItemSlot;
    public ItemSO[] itemSOs;

    private GameManager gameManager;
    public PlayerCtrl playerControl;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (playerControl == null)
        {
            playerControl = GameObject.Find("Player").GetComponent<PlayerCtrl>();
        }

        foreach (var slot in itemSlot) slot.EmptySlot();
        foreach (var slot in OrderItemSlot) slot.EmptySlot();
        foreach (var slot in SpawnItemSlot) slot.EmptySlot();
    }

    public bool useItem(string itemName)
    {
        foreach (var item in itemSOs)
        {
            if (item.itemName == itemName)
                return item.UseItem();
        }
        return false;
    }

    public int getItemID(string itemName)
    {
        foreach (var item in itemSOs)
        {
            if (item.itemName == itemName)
                return item.getItemID();
        }
        return 0;
    }

    public int[] getOrderItemIDs()
    {
        int[] ids = new int[OrderItemSlot.Length];
        for (int i = 0; i < OrderItemSlot.Length; i++)
        {
            ids[i] = getItemID(OrderItemSlot[i].itemName);
        }
        return ids;
    }

    public void clearOrderSlots()
    {
        foreach (var slot in OrderItemSlot) slot.EmptySlot();
    }

    public int addItem(string itemName, int quantity, Sprite itemSprite)
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            var slot = itemSlot[i];

            // Add to existing stack
            if (!slot.isFull && slot.itemName == itemName && !slot.isSpawn && !slot.isLocked)
            {
                Debug.Log($"📦 Stacking {itemName} in slot {i}");
                int leftOver = slot.addItem(itemName, quantity, itemSprite);
                return leftOver;
            }
        }

        for (int i = 0; i < itemSlot.Length; i++)
        {
            var slot = itemSlot[i];

            // Add to truly empty slot
            if (string.IsNullOrEmpty(slot.itemName) && !slot.isLocked && !slot.isSpawn)
            {
                Debug.Log($"🟢 Placing {itemName} in empty slot {i}");
                int leftOver = slot.addItem(itemName, quantity, itemSprite);
                return leftOver;
            }
        }

        Debug.LogError($"❌ Could not add item: {itemName} to any slot.");
        return quantity;
    }




    public int addOrderItem(string itemName, int quantity, Sprite itemSprite)
    {
        foreach (var slot in OrderItemSlot)
        {
            bool slotCanAccept = (!slot.isFull && slot.itemName == itemName) || (slot.quantity == 0 && !slot.isLocked && !slot.isSpawn);
            if (slotCanAccept)
            {
                int leftOver = slot.addItem(itemName, quantity, itemSprite);
                return leftOver;
            }
        }
        return quantity;
    }

    public int addSpawnItem(string itemName, int quantity, Sprite itemSprite)
    {
        foreach (var slot in SpawnItemSlot)
        {
            bool slotCanAccept = (!slot.isFull && slot.itemName == itemName) || (slot.quantity == 0 && !slot.isLocked && !slot.isSpawn);
            if (slotCanAccept)
            {
                int leftOver = slot.addItem(itemName, quantity, itemSprite);
                return leftOver;
            }
        }
        return quantity;
    }

    public void DeselectAllSlots()
    {
        foreach (var slot in itemSlot)
        {
            slot.selectedShader.SetActive(false);
            slot.thisItemSelected = false;
        }

        foreach (var slot in OrderItemSlot)
        {
            slot.selectedShader.SetActive(false);
            slot.thisItemSelected = false;
        }

        foreach (var slot in SpawnItemSlot)
        {
            slot.selectedShader.SetActive(false);
            slot.thisItemSelected = false;
        }
    }
}
