using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private Transform slotsParent;
    
    [Header("Events")]
    public UnityEvent<ItemSO> OnItemAdded;
    public UnityEvent<ItemSO> OnItemRemoved;
    
    private List<ItemSlot> slots = new List<ItemSlot>();
    private static InventoryManager instance;
    
    public static InventoryManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<InventoryManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        InitializeInventory();
    }

    private void InitializeInventory()
    {
        // Find all ItemSlot components under the slotsParent
        if (slotsParent != null)
        {
            slots.AddRange(slotsParent.GetComponentsInChildren<ItemSlot>());
        }
        else
        {
            // If no slots parent specified, find slots in the scene
            slots.AddRange(FindObjectsByType<ItemSlot>(FindObjectsSortMode.None));
        }

        if (slots.Count == 0)
        {
            Debug.LogWarning("No ItemSlots found in the scene!");
        }
    }

    public bool AddItem(ItemSO item)
    {
        if (item == null) return false;

        // Find first empty slot
        ItemSlot emptySlot = slots.Find(slot => slot.isEmpty);
        if (emptySlot != null)
        {
            emptySlot.SetItem(item);
            OnItemAdded?.Invoke(item);
            return true;
        }

        Debug.Log("Inventory is full!");
        return false;
    }

    public bool RemoveItem(ItemSO item)
    {
        if (item == null) return false;

        // Find slot containing the item
        ItemSlot slotWithItem = slots.Find(slot => slot.currentItem == item);
        if (slotWithItem != null)
        {
            slotWithItem.ClearSlot();
            OnItemRemoved?.Invoke(item);
            return true;
        }

        return false;
    }

    public bool HasItem(string itemName)
    {
        return slots.Exists(slot => !slot.isEmpty && slot.currentItem.itemName == itemName);
    }

    public ItemSO GetItem(string itemName)
    {
        ItemSlot slot = slots.Find(s => !s.isEmpty && s.currentItem.itemName == itemName);
        return slot?.currentItem;
    }

    public List<ItemSO> GetAllItems()
    {
        return slots.Where(slot => !slot.isEmpty).Select(slot => slot.currentItem).ToList();
    }

    public bool IsFull()
    {
        return !slots.Any(slot => slot.isEmpty);
    }
} 