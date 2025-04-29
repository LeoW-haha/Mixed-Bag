using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    private InventoryManager inventoryManager;
    private PlayerCtrl playerControl;
    private GameManager gameManager; 
    public GameObject SpawnSignal;
    public GameObject OrderSignal;
    public TMP_Text tutorialText;
    public int tutCount = 0;
    public float timer;
    private bool tutorialOver = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        playerControl = GameObject.Find("Player").GetComponent<PlayerCtrl>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (!gameManager.isWaterLevel) {
            SpawnSignal.SetActive(false);
            OrderSignal.SetActive(false);
            tutorialText.text = "WASD to Move";
        } else {

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (tutCount == 1) {
            tutorialText.text = "Hold Left Shift to Sprint";
        }
        if (tutCount == 2) {
            tutorialText.text = "Press E to open the inventory";
        }
        if (tutCount == 3) {
            tutorialText.text = "Items you get appear here, press Space on the Item Spawner";
            SpawnSignal.SetActive(true);
        }
        if (tutCount == 4) {
            tutorialText.text = "Right Click on the Items to put into your inventory, match them to the items on the top";
            SpawnSignal.SetActive(false);
        }
        if (tutCount == 5) {
            tutorialText.text = "Submit your order to the order menu";
            OrderSignal.SetActive(true);
        }
        if (tutCount == 6) {
            tutorialText.text = "Right click on the inventory to submit orders to the order menu and submit it";
            OrderSignal.SetActive(false);
        }
        if (tutCount == 7) {
            tutorialText.text = "Successful order! You can restock items with the money. Click on the arrows to switch orders";
        }
        if (tutCount == 100) {
            tutorialText.text = "Failed order! Try again.";
            timer += Time.deltaTime;
            if (timer >= 3) {
                tutCount = 6;
                timer = 0;
            }
        }
        if (tutCount == 8) {
            tutorialText.text = "You can have up to 3 unique orders at a time";
            timer += Time.deltaTime;
            if (timer >= 3) {
                tutCount = 9;
                timer = 0;
            }
        }
        if (tutCount == 9) {
            tutorialText.text = "Your energy is low, get a coffee from the spawn menu";
        }
        if (tutCount == 10 && !tutorialOver) {
            tutorialText.text = "That's about all, enjoy the game :)";
            timer += Time.deltaTime;
            if (timer >= 3) {
                tutorialText.text = "";
                timer = 0;
                tutorialOver = true;
            }
        }
        if (tutCount == 101) {
            tutorialText.text = "Grab the pipe fixer from the spawn inventory and fix the pipe";
        }
    }
}
