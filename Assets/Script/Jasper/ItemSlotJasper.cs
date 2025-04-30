using UnityEngine;
using UnityEngine.UI;

public class ItemSlotJasper : MonoBehaviour
{
    public ItemSOJasper currentItem;
    public Image itemImage;
    public bool isEmpty => currentItem == null;

    private void Start()
    {
        UpdateSlotUI();
    }

    public void SetItem(ItemSOJasper item)
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