using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;

public class InventoryManagerJasper : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private Transform slotsParent;
    
    [Header("Events")]
    public UnityEvent<ItemSOJasper> OnItemAdded;
    public UnityEvent<ItemSOJasper> OnItemRemoved;
    
    private List<ItemSlotJasper> slots = new List<ItemSlotJasper>();
    private static InventoryManagerJasper instance;
    
    public static InventoryManagerJasper Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<InventoryManagerJasper>();
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
        // Find all ItemSlotJasper components under the slotsParent
        if (slotsParent != null)
        {
            slots.AddRange(slotsParent.GetComponentsInChildren<ItemSlotJasper>());
        }
        else
        {
            // If no slots parent specified, find slots in the scene
            slots.AddRange(FindObjectsByType<ItemSlotJasper>(FindObjectsSortMode.None));
        }

        if (slots.Count == 0)
        {
            Debug.LogWarning("No ItemSlot found in the scene!");
        }
    }

    public bool AddItem(ItemSOJasper item)
    {
        if (item == null) return false;

        // Find first empty slot
        ItemSlotJasper emptySlot = slots.Find(slot => slot.isEmpty);
        if (emptySlot != null)
        {
            emptySlot.SetItem(item);
            OnItemAdded?.Invoke(item);
            return true;
        }

        Debug.Log("Inventory is full!");
        return false;
    }

    public bool RemoveItem(ItemSOJasper item)
    {
        if (item == null) return false;

        // Find slot containing the item
        ItemSlotJasper slotWithItem = slots.Find(slot => slot.currentItem == item);
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

    public ItemSOJasper GetItem(string itemName)
    {
        ItemSlotJasper slot = slots.Find(s => !s.isEmpty && s.currentItem.itemName == itemName);
        return slot?.currentItem;
    }

    public List<ItemSOJasper> GetAllItems()
    {
        return slots.Where(slot => !slot.isEmpty).Select(slot => slot.currentItem).ToList();
    }

    public bool IsFull()
    {
        return !slots.Any(slot => slot.isEmpty);
    }
} 