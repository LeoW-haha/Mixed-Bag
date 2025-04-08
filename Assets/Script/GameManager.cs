using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public float score = 0;         
    public Text scoreText;
    [SerializeField] private Image[] orderIcons;
    [SerializeField] private GameObject[] Items;
    private GameObject[] orderItems;
    public int maxOrderAmount;
    private InventoryManager inventoryManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int[] getOrderItemIDs() {
        int[] OrderItemIDs = new int[orderItems.Length];
        for (int i = 0; i < orderItems.Length; i++) {
            OrderItemIDs[i] = inventoryManager.getItemID(orderItems[i].GetComponent<Item>().itemName);
        }
        return OrderItemIDs;
    }

    public int compareOrderAndSupplied() {
        int differentItems = 0;
        int[] supply = inventoryManager.getOrderItemIDs();
        int[] order = this.getOrderItemIDs();
    
        var diff = supply.Except(order).ToArray();

        for (int i = 0; i < supply.Length; i++) {
            int peen = supply[i];
            for (int j = 0; j < diff.Length;j++) {
                if (peen == diff[j]) {
                    differentItems++;
                }
            }
        }
        Debug.Log(differentItems);
        return differentItems;
    }

    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        orderItems = new GameObject[orderIcons.Length];
        UpdateScoreUI();
        randomizeOrder();
    }
    public void AddScore(float points)
    {
        score += points;
        UpdateScoreUI();
    }
    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
    public void randomizeOrder() {
        orderItems = new GameObject[orderIcons.Length];
        for (int i = 0; i < orderIcons.Length; i++) {
            orderItems[i] = Items[Random.Range(0, Items.Length-1)];
            orderIcons[i].sprite = orderItems[i].GetComponent<Item>().sprite;
        }
    }

    public void calculateAndGiveScore() {
        float baseScore = (float)this.compareOrderAndSupplied();
        if (baseScore < 2) {
            this.AddScore(1/(baseScore+1));
        }
    }

    public void sendOrder() {
        calculateAndGiveScore();
        randomizeOrder();
        inventoryManager.clearOrderSlots();

        Time.timeScale = 1;
        inventoryManager.InventoryMenu.SetActive(false);
        inventoryManager.menuActivated = false;
        if(inventoryManager.orderMenuActivated) {
            inventoryManager.OrderMenu.SetActive(false);
            inventoryManager.orderMenuActivated = false;                
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
