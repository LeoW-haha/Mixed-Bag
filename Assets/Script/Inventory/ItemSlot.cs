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
    public bool isOrder;
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

    private void EmptySlot() {
        quantityText.enabled = false;
        itemImage.sprite = emptySprite;

        ItemDescriptionNameText.text = "";
        ItemDescriptionText.text = "";
        itemDescriptionImage.sprite = emptySprite;

        this.itemName = "";
        this.quantity = 0;
        this.itemSprite = emptySprite;
        this.isFull = false;
        this.itemDescription = "";

    }
    public void OnRightClick() {

        if (this.quantity>=1) {

            //Covers moving items from the inventory to the order menu
            if (inventoryManager.orderMenuActivated && !isOrder) {
                int leftOverItems = inventoryManager.addOrderItem(this.itemName, 1, this.itemSprite, this.itemDescription);

                if (leftOverItems == 0) {
                    this.isFull = false;
                    this.quantity-=1;
                    quantityText.text = this.quantity.ToString();
                    if (this.quantity <= 0) {
                        EmptySlot();
                    }
                }
            //vice versa
            } else if (inventoryManager.orderMenuActivated) {
                int leftOverItems = inventoryManager.addItem(this.itemName, 1, this.itemSprite, this.itemDescription);

                if (leftOverItems == 0) {
                    this.isFull = false;
                    this.quantity-=1;
                    quantityText.text = this.quantity.ToString();
                    if (this.quantity <= 0) {
                        EmptySlot();
                    }
                }
            //covers dropping items normally
            } else if (!isOrder) {
                //creates clone
                GameObject itemToDrop = new GameObject(itemName);
                Item newItem = itemToDrop.AddComponent<Item>();
                newItem.quantity = 1;
                newItem.itemName = itemName;
                newItem.sprite = itemSprite;
                newItem.itemDescription = itemDescription;

                //create and modify sr
                SpriteRenderer sr = itemToDrop.AddComponent<SpriteRenderer>();
                sr.sprite = itemSprite;
                sr.sortingOrder = 2;

                //Add a collider
                itemToDrop.AddComponent<BoxCollider2D>();

                //Add a rigidbody
                Rigidbody2D rb = itemToDrop.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0;


                //Set the location
                itemToDrop.transform.position = GameObject.FindWithTag("Player").transform.position + new Vector3(1f,0,0);
                itemToDrop.transform.localScale = new Vector3(0.7f,0.7f,1);

                //Subtracts the item
                this.quantity-=1;
                    quantityText.text = this.quantity.ToString();
                    if (this.quantity <= 0) {
                        EmptySlot();
                    }
            }

        }

    }



}
