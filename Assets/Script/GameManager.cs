using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class Order
{
    public GameObject[] orderItems; // First 2 are items, last is packaging colour visual
    public string requiredPackagingColour;
    public float orderTime;
    public float penaltyPoints;

    public Order(float orderTime, float penaltyPoints)
    {
        this.orderTime = orderTime;
        this.penaltyPoints = penaltyPoints;
    }
}

public class GameManager : MonoBehaviour
{
    public float score = 0;
    public Text scoreText;

    public float totalWeight = 0;
    public float maxWeight;

    [SerializeField] private Image[] orderIcons; // UI Icons (3 total)
    [SerializeField] private GameObject[] Items;
    private Order[] orders;
    public Order selectedOrder;
    public int maxOrderAmount;
    public int maxOrders;
    public int currentOrderIndex;

    private InventoryManager inventoryManager;
    public PlayerCtrl playerControl;
    private Notifier notifier;

    public TMP_Text topText;
    public Sprite emptySprite;
    private Timer shiftTimer;
    private Timer orderTimer;
    private Timer arriveTimer;
    public float orderTime;
    public float arriveTime;
    public float basePenaltyPoints;

    private bool disableButtons = false;
    public bool waitingForOrder = false;

    public GameObject pauseMenu;
    public GameObject endMenu;
    public bool pauseOn = false;
    public bool gameEnd = false;

    void Start()
    {
        pauseMenu.SetActive(false);
        endMenu.SetActive(false);
        Time.timeScale = 1;
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        shiftTimer = GameObject.Find("Timer").GetComponent<Timer>();
        orderTimer = GameObject.Find("OrderTimerText").GetComponent<Timer>();
        arriveTimer = GameObject.Find("GameManager").GetComponent<Timer>();
        orderTimer.timers = new float[maxOrderAmount];

        if (playerControl == null)
        {
            Debug.LogWarning("PlayerControl not assigned, trying to find...");
            playerControl = GameObject.Find("Player").GetComponent<PlayerCtrl>();
        }

        for (int i = 0; i < orderTimer.timers.Length; i++)
        {
            orderTimer.timers[i] = -1;
        }

        orders = new Order[maxOrderAmount];
        orders[0] = new Order(this.orderTime, basePenaltyPoints);
        orders[0].orderItems = new GameObject[orderIcons.Length];
        orderTimer.addTimer(0, orders[0].orderTime);
        this.selectedOrder = orders[0];
        currentOrderIndex = 0;
        notifier = GameObject.Find("NotificationHolder").GetComponent<Notifier>();
        randomizeOrders();
        topText.text = "Order: 1";
        updateTopBar(this.selectedOrder);
        UpdateScoreUI();
    }

    public void randomizeOrder(Order order)
    {
        for (int i = 0; i < orderIcons.Length - 1; i++)
        {
            order.orderItems[i] = Items[Random.Range(0, Items.Length)];
        }

        string[] colours = new string[] { "Green", "Black", "Red" };
        order.requiredPackagingColour = colours[Random.Range(0, colours.Length)];

        GameObject colourDummy = new GameObject("ColourDisplay");
        Item item = colourDummy.AddComponent<Item>();
        item.itemName = order.requiredPackagingColour;

        switch (order.requiredPackagingColour)
        {
            case "Green":
                item.sprite = Resources.Load<Sprite>("Sprites/GreenIcon");
                break;
            case "Black":
                item.sprite = Resources.Load<Sprite>("Sprites/BlackIcon");
                break;
            case "Red":
                item.sprite = Resources.Load<Sprite>("Sprites/RedIcon");
                break;
        }

        order.orderItems[orderIcons.Length - 1] = colourDummy;
    }

    public void randomizeOrders()
    {
        for (int i = 0; i < orders.Length; i++)
        {
            if (orders[i] != null)
            {
                randomizeOrder(orders[i]);
            }
        }
    }

    public void updateTopBar(Order order)
    {
        if (waitingForOrder) return;

        for (int i = 0; i < orderIcons.Length; i++)
        {
            if (order.orderItems[i] != null)
            {
                orderIcons[i].sprite = order.orderItems[i].GetComponent<Item>().sprite;
            }
            else
            {
                orderIcons[i].sprite = emptySprite;
            }
        }
    }

    public void calculateAndGiveScore()
    {
        float baseScore = (float)this.compareOrderAndSupplied();
        string playerColour = playerControl.currentPackagingColour;
        string requiredColour = selectedOrder.requiredPackagingColour;

        if (playerColour != requiredColour)
        {
            baseScore += 1f;
            Debug.Log("Wrong packaging colour! Player: " + playerColour + " | Required: " + requiredColour);
        }

        if (baseScore < 2)
        {
            this.AddScore(100 / (baseScore + 1));
        }
    }

    public Order[] getOrders()
    {
        return this.orders;
    }

    public void endTimer(bool isShift, bool isOrderCome, int i)
    {
        if (isShift)
        {
            endMenu.SetActive(true);
            Time.timeScale = 0.0f;
            gameEnd = true;
            return;
        }

        if (isOrderCome)
        {
            bool looking = true;
            for (int j = 0; j < orders.Length; j++)
            {
                if (orders[j] == null)
                {
                    while (looking)
                    {
                        orders[j] = new Order(this.orderTime, basePenaltyPoints);
                        orders[j].orderItems = new GameObject[orderIcons.Length];
                        orderTimer.addTimer(j, orders[j].orderTime);
                        randomizeOrder(orders[j]);

                        if (disableButtons || waitingForOrder)
                        {
                            disableButtons = false;
                            waitingForOrder = false;
                            selectedOrder = orders[j];
                            orderTimer.timerIndex = j;
                            currentOrderIndex = j;
                            updateTopBar(selectedOrder);
                        }

                        notifier.Notify("New order arrived");
                        looking = false;
                    }
                }
            }
            return;
        }

        int orderNumber = i + 1;
        notifier.Notify("Order Time " + orderNumber + " expired");
        minusScore(this.orders[i].penaltyPoints);
        randomizeOrder(this.orders[i]);

        if (i == currentOrderIndex)
        {
            updateTopBar(selectedOrder);
        }

        this.orders[i].orderTime = this.orderTime;
    }



    // All other unrelated methods (Update, Pause, etc.) stay unchanged

    public void AddScore(float points)
    {
        score += points;
        UpdateScoreUI();
    }

    public void minusScore(float points)
    {
        score -= points;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Money: $" + score;
        }
    }

    public int compareOrderAndSupplied()
    {
        int differentItems = 0;
        int[] supply = inventoryManager.getOrderItemIDs();
        int[] order = this.getOrderItemIDs(this.selectedOrder);
        var diff = supply.Except(order).ToArray();

        for (int i = 0; i < supply.Length; i++)
        {
            int peen = supply[i];
            for (int j = 0; j < diff.Length; j++)
            {
                if (peen == diff[j])
                {
                    differentItems++;
                }
            }
        }
        return differentItems;
    }

    public int[] getOrderItemIDs(Order order)
    {
        int[] OrderItemIDs = new int[orderIcons.Length - 1];
        for (int i = 0; i < OrderItemIDs.Length; i++)
        {
            OrderItemIDs[i] = inventoryManager.getItemID(order.orderItems[i].GetComponent<Item>().itemName);
        }
        return OrderItemIDs;
    }
}
