using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public ItemSlot[] itemSlot;
    public ItemSlot[] OrderItemSlot;
    public ItemSlot[] SpawnItemSlot;
    public ItemSO[] itemSOs;

    private GameManager gameManager;
    public PlayerCtrl playerControl;

    private List<ItemSlot> selectedItems = new List<ItemSlot>();

    public GameObject packagePrefab;
    public Sprite redPackageSprite;
    public Sprite greenPackageSprite;
    public Sprite blackPackageSprite;

    public float packagingDelay = 4f;

    [HideInInspector]
    public GameObject currentPackage;

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

    public void AddSelectedItem(ItemSlot slot)
    {
        if (!selectedItems.Contains(slot))
        {
            selectedItems.Add(slot);
            if (selectedItems.Count == 2)
            {
                StartPackaging();
            }
        }
    }

    public void RemoveSelectedItem(ItemSlot slot)
    {
        if (selectedItems.Contains(slot))
        {
            selectedItems.Remove(slot);
        }
    }

    public int addItem(string itemName, int quantity, Sprite itemSprite)
    {
        foreach (var slot in itemSlot)
        {
            if (!slot.isFull && slot.itemName == itemName && !slot.isSpawn && !slot.isLocked)
            {
                return slot.addItem(itemName, quantity, itemSprite);
            }
        }

        foreach (var slot in itemSlot)
        {
            if (string.IsNullOrEmpty(slot.itemName) && !slot.isLocked && !slot.isSpawn)
            {
                return slot.addItem(itemName, quantity, itemSprite);
            }
        }

        return quantity;
    }

    public int addOrderItem(string itemName, int quantity, Sprite itemSprite)
    {
        foreach (var slot in OrderItemSlot)
        {
            if ((!slot.isFull && slot.itemName == itemName) || (slot.quantity == 0 && !slot.isLocked && !slot.isSpawn))
            {
                return slot.addItem(itemName, quantity, itemSprite);
            }
        }
        return quantity;
    }

    public int addSpawnItem(string itemName, int quantity, Sprite itemSprite)
    {
        foreach (var slot in SpawnItemSlot)
        {
            if ((!slot.isFull && slot.itemName == itemName) || (slot.quantity == 0 && !slot.isLocked && !slot.isSpawn))
            {
                return slot.addItem(itemName, quantity, itemSprite);
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

    public void StartPackaging()
    {
        if (selectedItems.Count == 2)
        {
            StartCoroutine(SpawnPackageAfterDelay());
        }
    }

    private IEnumerator SpawnPackageAfterDelay()
    {
        string item1Name = selectedItems[0].itemName;
        string item2Name = selectedItems[1].itemName;
        string paintColor = playerControl.currentPackagingColour;

        yield return new WaitForSeconds(packagingDelay);

        foreach (var slot in selectedItems)
        {
            slot.EmptySlot();
        }
        selectedItems.Clear();

        GameObject package = Instantiate(packagePrefab, playerControl.transform.position, Quaternion.identity);
        currentPackage = package;

        var deliveryInfo = package.AddComponent<DeliveryPackage>();
        deliveryInfo.item1 = item1Name;
        deliveryInfo.item2 = item2Name;
        deliveryInfo.paintColor = paintColor;

        SpriteRenderer sr = package.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            switch (paintColor)
            {
                case "Red": sr.sprite = redPackageSprite; break;
                case "Green": sr.sprite = greenPackageSprite; break;
                case "Black": sr.sprite = blackPackageSprite; break;
                default: Debug.LogWarning("❌ Unknown paint color during packaging."); break;
            }
        }

        var pf = package.GetComponent<PackageFollower>();
        if (pf != null)
        {
            pf.target = playerControl.transform;
        }

        playerControl.hasPackage = true;
    }
}
