using UnityEngine;

public class SpawnMenuTrigger : MonoBehaviour, IInteractable
{
    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        if (playerControl == null)
        {
            Debug.LogWarning("PlayerControl not assigned, trying to find...");
            playerControl = GameObject.Find("Player").GetComponent<PlayerCtrl>();
        }
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();    
    }
    public bool CanInteract() {
        return true;
    }
    public void Interact() {
        if (!inventoryManager.menuActivated && !inventoryManager.ItemSpawnMenuActivated && !gameManager.gameEnd) {
            playerControl.canMove = false;
            inventoryManager.InventoryMenu.SetActive(true);
            inventoryManager.ItemSpawnMenu.SetActive(true);
            inventoryManager.menuActivated = true;
            inventoryManager.ItemSpawnMenuActivated = true;
        }
    }
    private InventoryManager inventoryManager;
    private GameManager gameManager;
    public PlayerCtrl playerControl;

    // Update is called once per frame
    void Update()
    {
        
    }
}
