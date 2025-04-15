using UnityEngine;

public class SpawnMenuTrigger : MonoBehaviour, IInteractable
{
    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }
    public bool CanInteract() {
        return true;
    }
    public void Interact() {
        if (!inventoryManager.menuActivated && !inventoryManager.ItemSpawnMenuActivated) {
            Time.timeScale = 0;
            inventoryManager.InventoryMenu.SetActive(true);
            inventoryManager.ItemSpawnMenu.SetActive(true);
            inventoryManager.menuActivated = true;
            inventoryManager.ItemSpawnMenuActivated = true;
        }
    }
    private InventoryManager inventoryManager;


    // Update is called once per frame
    void Update()
    {
        
    }
}
