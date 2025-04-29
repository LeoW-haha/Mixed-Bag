using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public ItemSO currentItem;
    public Image itemImage;
    public bool isEmpty => currentItem == null;

    private void Start()
    {
        UpdateSlotUI();
    }

    public void SetItem(ItemSO item)
    {
        currentItem = item;
        UpdateSlotUI();
    }

    public void ClearSlot()
    {
        currentItem = null;
        UpdateSlotUI();
    }

    private void UpdateSlotUI()
    {
        if (itemImage != null)
        {
            itemImage.sprite = currentItem?.itemSprite;
            itemImage.enabled = currentItem != null;
        }
    }
} 