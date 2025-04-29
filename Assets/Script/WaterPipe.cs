using UnityEngine;

public class WaterPipe : MonoBehaviour, IInteractable
{
    private GameManager gameManager;
    private InventoryManager inventoryManager;
    private PlayerCtrl playerControl;
    public bool isActive = false;
    public bool onCooldown=false;
    public float cooldownTimer;
    private float currentCooldown;
    public float flowRate = 0;
    public ParticleSystem waterSpray;
    private Notifier notifier;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        playerControl = GameObject.Find("Player").GetComponent<PlayerCtrl>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();  
        notifier = GameObject.Find("NotificationHolder").GetComponent<Notifier>();
        currentCooldown = cooldownTimer;      
    }
    public bool CanInteract() {
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive) {
            gameManager.waterLevel += flowRate*Time.deltaTime;
        }
        if (onCooldown) {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown <= 0) {
                onCooldown = false;
                currentCooldown = cooldownTimer;
            }
        }
    }

    public void activateWaterPipe() {
        if (!onCooldown) {
            waterSpray.Play();
            notifier.Notify("There's a pipe leak");
            isActive = true;
        }
    }
    public void deactivateWaterPipe() {
        waterSpray.Stop();
        isActive = false;
    }

    public void Interact() {
        if (inventoryManager.findAndUseItemSlot(5357) && isActive) {
            deactivateWaterPipe();
            notifier.Notify("Leak fixed");
            onCooldown = true;
            if (!gameManager.waterTutorialOver) {
                gameManager.FixedPipe();
                gameManager.waterLevel = 0;
            }
        } else if (!isActive) {
            notifier.Notify("Pipe not broken");
        } else if (!inventoryManager.findAndUseItemSlot(5357)) {
            notifier.Notify("No Leak Fixer");
        }
    }
}
