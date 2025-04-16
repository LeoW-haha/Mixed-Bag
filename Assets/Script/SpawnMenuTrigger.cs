using UnityEngine;

public class SpawnMenuTrigger : MonoBehaviour, IInteractable
{
    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        playerControl = GameObject.Find("Player").GetComponent<PlayerCtrl>();
    }
    public bool CanInteract() {
        return true;
    }
    public void Interact() {
        if (!inventoryManager.menuActivated && !inventoryManager.ItemSpawnMenuActivated) {
            playerControl.canMove = false;
            inventoryManager.InventoryMenu.SetActive(true);
            inventoryManager.ItemSpawnMenu.SetActive(true);
            inventoryManager.menuActivated = true;
            inventoryManager.ItemSpawnMenuActivated = true;
        }
    }
    private InventoryManager inventoryManager;
    private PlayerCtrl playerControl;

    // Update is called once per frame
    void Update()
    {
        
    }
}
