using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{

    //TIEM DATA

    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;

    //ITEM SLOT
    [SerializeField]
    private TMP_Text quantityText;
    [SerializeField]
    private Image itemImage;

    public void addItem(string itemName, int quantity, Sprite itemSprite) {
        this.itemName = itemName;
        this.quantity = quantity;
        this.itemSprite = itemSprite;
        isFull = true;

        quantityText.text = quantity.ToString();
        quantityText.enabled = true;
        itemImage.sprite = itemSprite;
        Debug.Log(itemSprite);
    }

    public void OnPointerClick(PointerEventData eventData) {
        throw new System.NotImplementedException();
    }
}
