using UnityEngine;

public class OrderMenu : MonoBehaviour, IInteractable
{
    void Start()
    {
        orderMenuID ??= GlobalHelper.GenerateUniqueID(gameObject);
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }
    public bool CanInteract() {
        return true;
    }
    public void Interact() {
        Debug.Log("Order");
        if (!inventoryManager.menuActivated && !inventoryManager.orderMenuActivated) {
            Time.timeScale = 1;
            inventoryManager.InventoryMenu.SetActive(true);
            inventoryManager.OrderMenu.SetActive(true);
            inventoryManager.menuActivated = true;
            inventoryManager.orderMenuActivated = true;
        }
    }
    private InventoryManager inventoryManager;
    public string orderMenuID {get; private set;}
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        
    }
}
