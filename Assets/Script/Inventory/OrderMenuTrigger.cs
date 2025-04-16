using UnityEngine;

public class OrderMenuTrigger : MonoBehaviour, IInteractable
{
    void Start()
    {
        orderMenuID ??= GlobalHelper.GenerateUniqueID(gameObject);
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        playerControl = GameObject.Find("Player").GetComponent<PlayerCtrl>();
    }
    public bool CanInteract() {
        return true;
    }
    public void Interact() {
        if (!inventoryManager.menuActivated && !inventoryManager.orderMenuActivated) {
            playerControl.canMove = false;
            inventoryManager.InventoryMenu.SetActive(true);
            inventoryManager.OrderMenu.SetActive(true);
            inventoryManager.menuActivated = true;
            inventoryManager.orderMenuActivated = true;
        }
    }
    private InventoryManager inventoryManager;
    public string orderMenuID {get; private set;}
    private PlayerCtrl playerControl;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        
    }
}
