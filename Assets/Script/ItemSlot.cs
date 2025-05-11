using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public bool isOrder;
    public bool isSpawn;
    public bool isLocked;
    public Sprite emptySprite;

    [SerializeField] private int maxNumber = 10;
    [SerializeField] private Text quantityText;
    [SerializeField] private Image slotImage;
    public GameObject selectedShader;
    public bool thisItemSelected;

    private InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    public int addItem(string itemName, int quantity, Sprite itemSprite)
    {
        if (isLocked) return quantity;

        // Accept if same item or slot is empty
        if (this.quantity == 0 || string.IsNullOrEmpty(this.itemName) || this.itemName == itemName)
        {
            this.itemName = itemName;
            this.itemSprite = itemSprite;
            if (slotImage != null) slotImage.sprite = itemSprite;

            this.quantity += quantity;

            if (this.quantity >= maxNumber)
            {
                int extra = this.quantity - maxNumber;
                this.quantity = maxNumber;
                if (quantityText != null)
                {
                    quantityText.text = this.quantity.ToString();
                    quantityText.enabled = true;
                }
                isFull = true;
                return extra;
            }

            if (quantityText != null)
            {
                quantityText.text = this.quantity.ToString();
                quantityText.enabled = true;
            }
            isFull = false;
            return 0;
        }

        // Reject if trying to add a different item
        return quantity;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            OnLeftClick();
        else if (eventData.button == PointerEventData.InputButton.Right)
            OnRightClick();
    }

    private void OnLeftClick()
    {
        if (thisItemSelected && inventoryManager.useItem(this.itemName) && !isSpawn && !isLocked)
        {
            quantity--;
            isFull = false;
            if (quantityText != null) quantityText.text = quantity.ToString();
            if (quantity <= 0) EmptySlot();
        }

        inventoryManager.DeselectAllSlots();
        if (selectedShader != null) selectedShader.SetActive(true);
        thisItemSelected = true;
    }

    private void OnRightClick()
    {
        if (quantity < 1) return;

        bool moved = false;

        //if (!isOrder && !isSpawn)
        //{
        //    moved = inventoryManager.addOrderItem(itemName, 1, itemSprite) == 0;
        //}
         if (!isOrder && isSpawn)
        {
            moved = inventoryManager.addSpawnItem(itemName, 1, itemSprite) == 0;
        }
        else if (isOrder || isSpawn)
        {
            moved = inventoryManager.addItem(itemName, 1, itemSprite) == 0;
        }

        if (moved)
        {
            quantity--;
            isFull = false;
            if (quantityText != null) quantityText.text = quantity.ToString();
            if (quantity <= 0) EmptySlot();
        }
    }

    public void EmptySlot()
    {
        if (!isSpawn)
        {
            if (quantityText != null) quantityText.enabled = false;
            if (slotImage != null) slotImage.sprite = emptySprite;
            itemName = string.Empty;
            itemSprite = null;
            quantity = 0;
            isFull = false;
        }
    }
}
