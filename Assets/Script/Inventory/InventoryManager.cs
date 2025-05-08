using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    public GameObject OrderMenu;
    public GameObject ItemSpawnMenu;
    public bool menuActivated = true;
    public bool orderMenuActivated = false;
    public bool ItemSpawnMenuActivated = false;

    public ItemSlot[] itemSlot;
    public ItemSlot[] OrderItemSlot;
    public ItemSlot[] SpawnItemSlot;
    public ItemSO[] itemSOs;
    public TMP_Text totalWeightText;

    private Text controlText;
    private string standardText;
    [TextArea]
    public string inventoryText;

    private GameManager gameManager;
    public PlayerCtrl playerControl;

    void Start()
    {
        InventoryMenu.SetActive(true);
        OrderMenu.SetActive(true);
        ItemSpawnMenu.SetActive(true);

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (playerControl == null)
        {
            playerControl = GameObject.Find("Player").GetComponent<PlayerCtrl>();
        }

        controlText = GameObject.Find("ControlText").GetComponent<Text>();
        standardText = controlText.text;
    }

    void Update()
    {
        totalWeightText.text = "Total Weight: " + gameManager.totalWeight.ToString() + "/" + gameManager.maxWeight.ToString();
        totalWeightText.color = gameManager.totalWeight > gameManager.maxWeight ? Color.red : Color.white;
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

    public float getWeight(string itemName)
    {
        foreach (var item in itemSOs)
        {
            if (item.itemName == itemName)
                return item.weight;
        }
        return 0f;
    }

    public float getTotalWeight()
    {
        float total = 0f;
        foreach (var slot in itemSlot)
        {
            total += getWeight(slot.itemName) * slot.quantity;
        }
        return total;
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
        foreach (var slot in OrderItemSlot)
        {
            slot.EmptySlot();
        }
    }

    public bool unlockSlots(int number)
    {
        int count = 0;
        foreach (var slot in itemSlot)
        {
            if (slot.isLocked && count < number)
            {
                slot.isLocked = false;
                slot.itemDescriptionImage.sprite = slot.emptySprite;
                slot.itemImage.sprite = slot.emptySprite;
                slot.itemSprite = slot.emptySprite;
                count++;
            }
        }
        return count > 0;
    }

    public int addItem(string itemName, int quantity, Sprite itemSprite, string itemDescription, float restockCost)
    {
        foreach (var slot in itemSlot)
        {
            if ((!slot.isFull && slot.itemName == itemName || slot.quantity == 0) && !slot.isSpawn && !slot.isLocked)
            {
                int leftOver = slot.addItem(itemName, quantity, itemSprite, itemDescription, restockCost);
                if (leftOver > 0)
                    return addItem(itemName, leftOver, itemSprite, itemDescription, restockCost);
                return 0;
            }
        }
        return quantity;
    }

    public int addOrderItem(string itemName, int quantity, Sprite itemSprite, string itemDescription, float restockCost)
    {
        foreach (var slot in OrderItemSlot)
        {
            if ((!slot.isFull && slot.itemName == itemName || slot.quantity == 0) && !slot.isSpawn && !slot.isLocked)
            {
                int leftOver = slot.addItem(itemName, quantity, itemSprite, itemDescription, restockCost);
                if (leftOver > 0)
                    return addOrderItem(itemName, leftOver, itemSprite, itemDescription, restockCost);
                return 0;
            }
        }
        return quantity;
    }

    public int addSpawnItem(string itemName, int quantity, Sprite itemSprite, string itemDescription, float restockCost)
    {
        foreach (var slot in SpawnItemSlot)
        {
            if ((!slot.isFull && slot.itemName == itemName || slot.quantity == 0) && !slot.isSpawn && !slot.isLocked)
            {
                int leftOver = slot.addItem(itemName, quantity, itemSprite, itemDescription, restockCost);
                if (leftOver > 0)
                    return addSpawnItem(itemName, leftOver, itemSprite, itemDescription, restockCost);
                return 0;
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