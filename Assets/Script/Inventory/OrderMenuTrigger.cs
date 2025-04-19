using UnityEngine;

public class OrderMenuTrigger : MonoBehaviour, IInteractable
{
    void Start()
    {
        orderMenuID ??= GlobalHelper.GenerateUniqueID(gameObject);
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        playerControl = GameObject.Find("Player").GetComponent<PlayerCtrl>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    public bool CanInteract() {
        return true;
    }
    public void Interact() {
        if (!inventoryManager.menuActivated && !inventoryManager.orderMenuActivated && !gameManager.gameEnd) {
            playerControl.canMove = false;
            inventoryManager.InventoryMenu.SetActive(true);
            inventoryManager.OrderMenu.SetActive(true);
            inventoryManager.menuActivated = true;
            inventoryManager.orderMenuActivated = true;
        }
    }
    private InventoryManager inventoryManager;
    private GameManager gameManager;

    public string orderMenuID {get; private set;}
    private PlayerCtrl playerControl;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        
    }
}
