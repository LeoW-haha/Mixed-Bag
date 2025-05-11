using UnityEngine;

public class SpawnMenuTrigger : MonoBehaviour, IInteractable
{
    private InventoryManager inventoryManager;
    private GameManager gameManager;
    public PlayerCtrl playerControl;

    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        //playerControl = GameObject.Find("Player").GetComponent<PlayerCtrl>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        // You can insert simplified logic here if needed, or remove if no longer used
    }
}
