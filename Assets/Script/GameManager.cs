using UnityEngine;
using UnityEngine.UI;
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
    [Header("Gameplay Settings")]
    public float score = 0;
    public float totalWeight = 0;
    public float maxWeight;
    public float orderTime;
    public float arriveTime;
    public float basePenaltyPoints;

    [Header("UI")]
    public Text scoreText;
    public TMP_Text topText;
    public Sprite emptySprite;
    [SerializeField] private Image[] orderIcons;

    [Header("Game State")]
    private Order[] orders;
    public Order selectedOrder;
    public int maxOrderAmount;
    public int currentOrderIndex;
    public bool waitingForOrder = false;
    private bool disableButtons = false;
    public bool pauseOn = false;
    public bool gameEnd = false;

    [Header("References")]
    [SerializeField] private GameObject[] Items;
    public PlayerCtrl playerControl;
    private InventoryManager inventoryManager;
    private Notifier notifier;
    private Timer shiftTimer, orderTimer, arriveTimer;

    [Header("UI Panels")]
    public GameObject pauseMenu;
    public GameObject endMenu;

    [Header("Spawn Points & Paths")]
    public Transform spawnLocation1;
    public Transform spawnLocation2;
    public Transform wayPoint1;
    public Transform wayPoint2;
    public Transform wayPoint3;
    public Transform wayPoint4;

    void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        pauseMenu.SetActive(false);
        endMenu.SetActive(false);
        Time.timeScale = 1;

        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        shiftTimer = GameObject.Find("Timer").GetComponent<Timer>();
        orderTimer = GameObject.Find("OrderTimerText").GetComponent<Timer>();
        arriveTimer = GameObject.Find("GameManager").GetComponent<Timer>();
        notifier = GameObject.Find("NotificationHolder").GetComponent<Notifier>();

        if (playerControl == null)
        {
            playerControl = GameObject.Find("Player").GetComponent<PlayerCtrl>();
        }

        orderTimer.timers = new float[maxOrderAmount];
        for (int i = 0; i < orderTimer.timers.Length; i++)
            orderTimer.timers[i] = -1;

        orders = new Order[maxOrderAmount];
        orders[0] = new Order(orderTime, basePenaltyPoints) { orderItems = new GameObject[orderIcons.Length] };
        selectedOrder = orders[0];
        currentOrderIndex = 0;
        orderTimer.addTimer(0, selectedOrder.orderTime);

        randomizeOrders();
        topText.text = "Order: 1";
        updateTopBar(selectedOrder);
        UpdateScoreUI();
    }

    public void randomizeOrder(Order order)
    {
        for (int i = 0; i < orderIcons.Length - 1; i++)
            order.orderItems[i] = Items[Random.Range(0, Items.Length)];

        string[] colours = { "Green", "Black", "Red" };
        order.requiredPackagingColour = colours[Random.Range(0, colours.Length)];

        GameObject colourDummy = new GameObject("ColourDisplay");
        Item item = colourDummy.AddComponent<Item>();
        item.itemName = order.requiredPackagingColour;
        item.sprite = Resources.Load<Sprite>($"Sprites/{order.requiredPackagingColour}Icon");
        order.orderItems[orderIcons.Length - 1] = colourDummy;

        SpawnConveyorItem(order.orderItems[0], spawnLocation1.position, wayPoint1, wayPoint2);
        SpawnConveyorItem(order.orderItems[1], spawnLocation2.position, wayPoint3, wayPoint4);
    }

    private void SpawnConveyorItem(GameObject prefab, Vector3 spawnPos, Transform wp1, Transform wp2)
    {
        GameObject item = Instantiate(prefab, spawnPos, Quaternion.identity);
        ConveyorMover mover = item.GetComponent<ConveyorMover>();
        if (mover != null)
        {
            mover.SetPath(wp1, wp2);
        }
    }

    public void randomizeOrders()
    {
        foreach (var order in orders)
        {
            if (order != null)
                randomizeOrder(order);
        }
    }

    public void updateTopBar(Order order)
    {
        if (waitingForOrder) return;

        for (int i = 0; i < orderIcons.Length; i++)
        {
            var item = order.orderItems[i];
            orderIcons[i].sprite = item != null ? item.GetComponent<Item>().sprite : emptySprite;
        }
    }

    public void calculateAndGiveScore()
    {
        float baseScore = compareOrderAndSupplied();
        if (playerControl.currentPackagingColour != selectedOrder.requiredPackagingColour)
        {
            baseScore++;
            Debug.Log($"Wrong packaging colour! Player: {playerControl.currentPackagingColour} | Required: {selectedOrder.requiredPackagingColour}");
        }

        if (baseScore < 2)
            AddScore(100 / (baseScore + 1));
    }

    public Order[] getOrders() => orders;

    public void endTimer(bool isShift, bool isOrderCome, int i)
    {
        if (isShift)
        {
            endMenu.SetActive(true);
            Time.timeScale = 0f;
            gameEnd = true;
            return;
        }

        if (isOrderCome)
        {
            for (int j = 0; j < orders.Length; j++)
            {
                if (orders[j] == null)
                {
                    orders[j] = new Order(orderTime, basePenaltyPoints) { orderItems = new GameObject[orderIcons.Length] };
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
                    break;
                }
            }
            return;
        }

        notifier.Notify($"Order Time {i + 1} expired");
        minusScore(orders[i].penaltyPoints);
        randomizeOrder(orders[i]);
        if (i == currentOrderIndex)
            updateTopBar(selectedOrder);

        orders[i].orderTime = orderTime;
    }

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

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Money: ${score}";
    }

    public int compareOrderAndSupplied()
    {
        int[] supply = inventoryManager.getOrderItemIDs();
        int[] orderIDs = getOrderItemIDs(selectedOrder);
        return supply.Except(orderIDs).Count();
    }

    public int[] getOrderItemIDs(Order order)
    {
        return order.orderItems.Take(orderIcons.Length - 1)
                                .Select(item => inventoryManager.getItemID(item.GetComponent<Item>().itemName))
                                .ToArray();
    }
}