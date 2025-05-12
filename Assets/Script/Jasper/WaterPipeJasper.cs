using UnityEngine;
using UnityEngine.UI;

public class WaterPipeJasper : MonoBehaviour
{
    private bool isActive = false;
    private bool onCooldown=false;
    private Color originalColor;
    [SerializeField] private Color highlightColor = new Color(0.5f, 1f, 0.5f, 0.7f);
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private ParticleSystem waterSpray;
    [SerializeField] private float cooldownTimer;
    [SerializeField] private float flowRate;
    [SerializeField] private float leakChance;
    [SerializeField] private float drainRate;
    [SerializeField] private Image WaterBar;
    [SerializeField] private GameObject WaterSurface;
    [SerializeField] private GameObject WaterSurfaceCollider;
    private float waterLevel;
    private GameManagerJasper gameManager;
    private GameObject playerInRange;
    private bool isPlayerInRange = false;
    public bool complete = false;
    private float currentCooldown;
    private PlayerController player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerJasper>(); 
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        currentCooldown = cooldownTimer;
        InvokeRepeating("randomPipeLeak", 1.0f, 1.0f);     
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            FixPipe();
        } 
        if (isActive) {
            waterLevel += flowRate*Time.deltaTime;
        } else {
            waterLevel -= drainRate*Time.deltaTime;
        }
        if (waterLevel > 100) {
            waterLevel = 100;
        }
        if (waterLevel < 0) {
            waterLevel = 0;
        }
        WaterBar.fillAmount = waterLevel/100;
        if (waterLevel >= 100) {
            WaterSurface.SetActive(true);
            WaterSurfaceCollider.SetActive(true);
        } else {
            WaterSurface.SetActive(false);
            WaterSurfaceCollider.SetActive(false);
        }
        if (onCooldown) {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown <= 0) {
                currentCooldown = cooldownTimer;
                onCooldown = false;
            } 
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerInRange = other.gameObject;
            
            // Highlight Sprite
            if (spriteRenderer != null)
            {
                spriteRenderer.color = highlightColor;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerInRange = null;
            
            // Reset sprite
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
        }
    }

    public void randomPipeLeak() {
        if(Random.Range(0, 100) < leakChance && !onCooldown) {
            activateWaterPipe();
        }
    }

    public void activateWaterPipe() {
        if (!onCooldown) {
            waterSpray.Play();
            isActive = true;
        }
    }

    private void FixPipe() {
        if (!isPlayerInRange || playerInRange == null) return;

        PlayerController playerController = playerInRange.GetComponent<PlayerController>();
        if (playerController == null || !playerController.IsCarryingItem()) return;

        GameObject carriedItem = playerController.GetCarriedItem();
        CollectibleItem item = carriedItem.GetComponent<CollectibleItem>();

        //Check if item is Pipe FIxer
        if (item.GetItemName() == "Pipe Fixer")
        {
            waterSpray.Stop();
            isActive = false;
            onCooldown = true;
            Destroy(carriedItem);
            player.DropItem();
            if (!complete) {
                complete = true;
            }
        }
        else
        {
            Debug.Log("No item equipped");
        }        
    }
}
