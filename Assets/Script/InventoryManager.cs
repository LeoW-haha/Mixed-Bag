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
        for (int i = 0; i < itemSlot.Length; i++)
        {
            var slot = itemSlot[i];

            // Add to existing stack
            if (!slot.isFull && slot.itemName == itemName && !slot.isSpawn && !slot.isLocked)
            {

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

                int leftOver = slot.addItem(itemName, quantity, itemSprite);
                return leftOver;
            }
        }

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

    public void StartPackaging()
    {
        if (selectedItems.Count == 2)
        {
            Debug.Log($"✅ Packaging started with: {selectedItems[0].itemName} and {selectedItems[1].itemName} using paint: {playerControl.currentPackagingColour}");
            StartCoroutine(SpawnPackageAfterDelay());
        }
    }

    private IEnumerator SpawnPackageAfterDelay()
    {
        Debug.Log($"✅ Packaging started with: {selectedItems[0].itemName} and {selectedItems[1].itemName} using paint: {playerControl.currentPackagingColour}");

        yield return new WaitForSeconds(packagingDelay);

        foreach (var slot in selectedItems)
        {
            slot.EmptySlot();
        }
        selectedItems.Clear();

        GameObject package = Instantiate(packagePrefab, playerControl.transform.position, Quaternion.identity);
        SpriteRenderer sr = package.GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            switch (playerControl.currentPackagingColour)
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

        currentPackage = package;
        playerControl.hasPackage = true;
    }

}
