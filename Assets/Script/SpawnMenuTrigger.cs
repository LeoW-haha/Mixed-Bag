using UnityEngine;

public class SpawnMenuTrigger : MonoBehaviour, IInteractable
{
    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        playerControl = GameObject.Find("Player").GetComponent<PlayerCtrl>();
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
            gameManager.OpenedSpawn();
        }
    }
    private InventoryManager inventoryManager;
    private GameManager gameManager;
    private PlayerCtrl playerControl;

    // Update is called once per frame
    void Update()
    {
        
    }
}
