using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{

    //TIEM DATA

    public string itemName;
    public int quantity;
    private Sprite itemSprite;
    public bool isFull;
    public string itemDescription;
    public Sprite emptySprite;
    [SerializeField]
    private int maxNumber;


    //ITEM SLOT
    [SerializeField]
    private TMP_Text quantityText;

    //ITEM DESCRIPTION SLOT
    public Image itemDescriptionImage;
    public TMP_Text ItemDescriptionNameText;
    public TMP_Text ItemDescriptionText;

    public Image itemImage;
    public GameObject selectedShader;
    public bool thisItemSelected;
    private InventoryManager inventoryManager;

    private void Start() {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    public int addItem(string itemName, int quantity, Sprite itemSprite, string itemDescription) {
        //Check to see if slot full
        if (isFull) {
            return quantity;
        }

        //Change Name
        this.itemName = itemName;

        //Change Picture
        this.itemSprite = itemSprite;
        itemImage.sprite = itemSprite;

        //Change Description
        this.itemDescription = itemDescription;

        //Change Quantity
        this.quantity += quantity;
        if (this.quantity >= maxNumber) {
            quantityText.text = maxNumber.ToString();
            quantityText.enabled = true;
            isFull = true;        

            //Return Leftovers
            int extraItems = this.quantity - maxNumber;
            this.quantity = maxNumber;
            return extraItems;
        }

        //Update Quantity Text
        quantityText.text = this.quantity.ToString();
        quantityText.enabled = true;        
        return 0;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            OnLeftClick();
        }
        if (eventData.button == PointerEventData.InputButton.Right) {
            OnRightClick();
        }
    }

    public void OnLeftClick() {
        if(thisItemSelected) {
            Debug.Log(this.itemName);
            Debug.Log(inventoryManager.getItemID(this.itemName));
        }
        inventoryManager.DeselectAllSlots();
        selectedShader.SetActive(true);
        thisItemSelected = true;
        ItemDescriptionNameText.text = itemName;
        ItemDescriptionText.text = itemDescription;
        itemDescriptionImage.sprite = itemSprite;
        if(itemDescriptionImage.sprite == null) {
            itemDescriptionImage.sprite = emptySprite;
        }
    }
    public void OnRightClick() {

    }
}
