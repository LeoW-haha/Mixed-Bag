using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.Collections;

public class Order
{
    public bool isCompleted;
    public GameObject[] orderItems; // First 2 are items, last is packaging colour visual
    public string requiredPackagingColour;
    public float orderTime;
    public float penaltyPoints;

    public Order(float orderTime, float penaltyPoints)
    {
        this.orderTime = orderTime;
        this.penaltyPoints = penaltyPoints;
        isCompleted = false;
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
    [SerializeField] private Image[] orderIcons2; // Bottom canvas



    void Start()
    {
        InitializeGame();

        Invoke(nameof(CreateSecondOrder), 15f);
    }

    [Header("Delivery Conveyor Setup")]
    public Transform deliveryZone1;
    public Transform deliveryZone2;
    public Transform deliveryWaypoint1;
    public Transform deliveryWaypoint2;
    public Transform deliveryWaypoint3;
    public Transform deliveryWaypoint4;

    public Sprite tickSprite;  // assign in Inspector
    public Sprite xSprite;
    [Header("Delivery Result System")]
    public DeliveryFeedbackUI floatingFeedback; // Drag the TickSlot UI object that has the script
    public Transform tickSlotContainer; // Parent object that holds TickSlots
    private int currentTickSlotIndex = 0;

    public float spawnInterval = 5f;  // Default value
    public int itemsPerSpawn = 3;

    [Header("Order UI Containers")]
    public GameObject orderBox1; // Top UI box
    public GameObject orderBox2; // Second UI box


    private void CreateSecondOrder()
    {
        if (orders.Length > 1 && orders[1] == null)
        {
            orders[1] = new Order(orderTime, basePenaltyPoints) { orderItems = new GameObject[orderIcons.Length] };
            orderTimer.addTimer(1, orders[1].orderTime);
            randomizeOrder(orders[1], 1); // Show in second canvas
            notifier.Notify("Order 2 has arrived!");

            Debug.Log("🟢 Order 2 created and randomized.");
            Debug.Log($"Order 2 item1: {orders[1].orderItems[0]?.GetComponent<Item>()?.itemName}");
            Debug.Log($"Order 2 item2: {orders[1].orderItems[1]?.GetComponent<Item>()?.itemName}");
        }
    }



    private void ClearOrderIcons(Image[] iconSet)
    {
        foreach (var icon in iconSet)
        {
            if (icon != null)
            {
                icon.sprite = emptySprite;
            }
        }
    }
    public void VerifyDelivery(DeliveryPackage delivered)
    {
        bool matched = false;

        for (int i = 0; i < orders.Length; i++)
        {
            if (orders[i] == null) continue;

            string orderColor = orders[i].requiredPackagingColour;
            var requiredItems = orders[i].orderItems;

            string required1 = requiredItems[0].GetComponent<Item>().itemName;
            string required2 = requiredItems[1].GetComponent<Item>().itemName;

            bool colorMatch = delivered.paintColor == orderColor;
            bool item1Match = delivered.item1 == required1 || delivered.item1 == required2;
            bool item2Match = delivered.item2 == required1 || delivered.item2 == required2;
            bool distinct = delivered.item1 != delivered.item2;

            bool itemsMatch = item1Match && item2Match && distinct;

            if (colorMatch && itemsMatch)
            {
                matched = true;
                orders[i].isCompleted = true;

                // ✅ Hide UI box and log
                if (i == 0 && orderIcons != null)
                {
                    ClearOrderIcons(orderIcons);
                    Debug.Log($"✅ Delivery matched Order {i}, cleared top order icons.");
                }
                else if (i == 1 && orderIcons2 != null)
                {
                    ClearOrderIcons(orderIcons2);
                    Debug.Log($"✅ Delivery matched Order {i}, cleared bottom order icons.");
                }


                break;
            }
        }



        // Show tick/cross sprite
        GameObject feedback = new GameObject("DeliveryFeedback");
        SpriteRenderer sr = feedback.AddComponent<SpriteRenderer>();
        sr.sprite = matched ? tickSprite : xSprite;
        sr.sortingOrder = 10;
        feedback.transform.position = deliveryZone1.position + Vector3.up * 2;
        Destroy(feedback, 2f);

        // Update canvas tick slot
        if (tickSlotContainer != null && currentTickSlotIndex < tickSlotContainer.childCount)
        {
            Transform slot = tickSlotContainer.GetChild(currentTickSlotIndex);
            Image img = slot.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = matched ? tickSprite : xSprite;
                img.enabled = true;
            }
            currentTickSlotIndex++;
        }
    }



    private IEnumerator ReenableOrderBox(GameObject box, int orderIndex)
    {
        float waitTime = orderTimer.timers[orderIndex];

        // Wait until this order’s timer runs out
        while (waitTime > 0)
        {
            waitTime -= Time.deltaTime;
            yield return null;
        }

        if (box != null)
        {
            box.SetActive(true);
        }
    }






    public void ShowDeliveryFeedback(bool success)
    {
        // ✅ Show floating tick/cross
        if (floatingFeedback != null)
        {
            floatingFeedback.ShowResult(success);
        }

        // ✅ Show permanent tick/cross in canvas slot
        if (tickSlotContainer != null && currentTickSlotIndex < tickSlotContainer.childCount)
        {
            Transform slot = tickSlotContainer.GetChild(currentTickSlotIndex);
            Image image = slot.GetComponent<Image>();

            if (image != null)
            {
                image.sprite = success ? floatingFeedback.tickSprite : floatingFeedback.crossSprite;
                image.enabled = true;
            }

            currentTickSlotIndex++;
        }
    }




    public void SendToDeliveryConveyor(GameObject package, int deliveryZone)
    {
        switch (deliveryZone)
        {
            case 1:
                MoveExistingPackageToConveyor(package, deliveryZone1.position, deliveryWaypoint1, deliveryWaypoint2);
                break;
            case 2:
                MoveExistingPackageToConveyor(package, deliveryZone2.position, deliveryWaypoint3, deliveryWaypoint4);
                break;
            default:
                Debug.LogWarning("Invalid delivery zone selected.");
                break;
        }
    }

    private void MoveExistingPackageToConveyor(GameObject package, Vector3 position, Transform wp1, Transform wp2)
    {
        package.transform.position = position; // Move existing package

        ConveyorMover mover = package.GetComponent<ConveyorMover>();
        if (mover == null)
        {
            mover = package.AddComponent<ConveyorMover>();
        }

        mover.SetPath(wp1, wp2);
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

        // ✅ Only create Order 0 at start
        // ✅ Create both orders at start
        orders[0] = new Order(orderTime, basePenaltyPoints) { orderItems = new GameObject[orderIcons.Length] };
        orders[1] = new Order(orderTime, basePenaltyPoints) { orderItems = new GameObject[orderIcons.Length] };
        selectedOrder = orders[0];
        currentOrderIndex = 0;

        orderTimer.addTimer(0, orders[0].orderTime);
        orderTimer.addTimer(1, orders[1].orderTime);
        randomizeOrder(orders[0], 0); // Top canvas
        randomizeOrder(orders[1], 1); // Bottom canvas


        topText.text = "Order: 1";
        updateTopBar(selectedOrder);
        UpdateScoreUI();
    }


    public void randomizeOrder(Order order, int orderIndex = 0)
    {
        List<GameObject> chosenItems = new List<GameObject>();

        while (chosenItems.Count < orderIcons.Length - 1)
        {
            GameObject candidate = Items[Random.Range(0, Items.Length)];
            if (!chosenItems.Contains(candidate))
                chosenItems.Add(candidate);
        }

        for (int i = 0; i < chosenItems.Count; i++)
        {
            order.orderItems[i] = chosenItems[i];
        }

        string[] colours = { "Green", "Black", "Red" };
        order.requiredPackagingColour = colours[Random.Range(0, colours.Length)];

        GameObject colourDummy = new GameObject("ColourDisplay");
        Item item = colourDummy.AddComponent<Item>();
        item.itemName = order.requiredPackagingColour;
        item.sprite = Resources.Load<Sprite>($"Sprites/{order.requiredPackagingColour}Icon");
        order.orderItems[orderIcons.Length - 1] = colourDummy;

        Image[] icons = orderIndex == 0 ? orderIcons : orderIcons2;


        for (int i = 0; i < icons.Length; i++)
        {
            var entry = order.orderItems[i];
            icons[i].sprite = entry != null ? entry.GetComponent<Item>().sprite : emptySprite;
        }

        Debug.Log($"🎲 Order {orderIndex} randomized: {order.orderItems[0]?.GetComponent<Item>()?.itemName}, {order.orderItems[1]?.GetComponent<Item>()?.itemName}, {order.requiredPackagingColour}");

        SpawnConveyorItemMultiple(order.orderItems[0], spawnLocation1.position, wayPoint1, wayPoint2);
        SpawnConveyorItemMultiple(order.orderItems[1], spawnLocation2.position, wayPoint3, wayPoint4);

    }

    private void SpawnConveyorItemMultiple(GameObject prefab, Vector3 spawnPos, Transform wp1, Transform wp2)
    {
        StartCoroutine(SpawnItemsWithDelay(prefab, spawnPos, wp1, wp2, itemsPerSpawn, spawnInterval));
    }

    private IEnumerator SpawnItemsWithDelay(GameObject prefab, Vector3 spawnPos, Transform wp1, Transform wp2, int quantity, float interval)
    {
        for (int i = 0; i < quantity; i++)
        {
            GameObject item = Instantiate(prefab, spawnPos, Quaternion.identity);
            ConveyorMover mover = item.GetComponent<ConveyorMover>();
            if (mover != null)
            {
                mover.SetPath(wp1, wp2);
            }

            yield return new WaitForSeconds(interval);
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
            Debug.Log("🔴 Shift ended. Game over.");
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
                    orders[j].isCompleted = false;
                    randomizeOrder(orders[j], j);
                    notifier.Notify("New order arrived");
                    Debug.Log($"🟢 Order {j} created after order come.");
                    return;
                }
            }
        }

        if (orders[i] != null && orders[i].isCompleted)
        {
            Debug.Log($"✅ Order {i} was completed. Replacing with new.");
        }
        else
        {
            notifier.Notify($"Order Time {i + 1} expired");
            minusScore(orders[i].penaltyPoints);
            Debug.Log($"⚠️ Order {i} expired. Replacing with new.");
        }

        // Replace with new order
        orders[i] = new Order(orderTime, basePenaltyPoints) { orderItems = new GameObject[orderIcons.Length] };
        orderTimer.addTimer(i, orderTime);
        randomizeOrder(orders[i], i);

        // Re-enable corresponding canvas box
        if (i == 0 && orderBox1 != null)
        {
            orderBox1.SetActive(true);
            Debug.Log("🟢 Re-enabled orderBox1");
        }
        else if (i == 1 && orderBox2 != null)
        {
            orderBox2.SetActive(true);
            Debug.Log("🟢 Re-enabled orderBox2");
        }

        if (i == currentOrderIndex)
            updateTopBar(orders[i]);
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