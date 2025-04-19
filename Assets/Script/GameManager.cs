using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class Order {
    public GameObject[] orderItems;
    public float orderTime;
    public float penaltyPoints;

    public Order(float orderTime, float penaltyPoints) {
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

    [SerializeField] private Image[] orderIcons;
    [SerializeField] private GameObject[] Items;
    private Order[] orders;
    public Order selectedOrder;
    public int maxOrderAmount;
    public int maxOrders;
    public int currentOrderIndex;

    private InventoryManager inventoryManager;
    private PlayerCtrl playerControl;
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
        playerControl = GameObject.Find("Player").GetComponent<PlayerCtrl>();
        for (int i = 0; i<orderTimer.timers.Length; i++) {
            orderTimer.timers[i] = -1;
        }

        orders = new Order[maxOrderAmount];

        orders[0] = new Order(this.orderTime, basePenaltyPoints);
        orders[0].orderItems = new GameObject[orderIcons.Length];
        orderTimer.addTimer(0, orders[0].orderTime);

        this.selectedOrder = new Order(this.orderTime, basePenaltyPoints);
        this.selectedOrder = orders[0];
        currentOrderIndex = 0;
        notifier = GameObject.Find("NotificationHolder").GetComponent<Notifier>();
        randomizeOrders();
        topText.text = "Order: 1";
        updateTopBar(this.selectedOrder);
        UpdateScoreUI();
    }

    // Update is called once per frame
    void Update()
    {
        this.totalWeight = inventoryManager.getTotalWeight();
        if (Input.GetButtonDown("Pause") && !pauseOn && !gameEnd) {
            pauseMenu.SetActive(true);
            Time.timeScale = 0.0f;
            pauseOn = true;
        } else if (Input.GetButtonDown("Pause") && pauseOn && !gameEnd) {
            pauseMenu.SetActive(false);
            if (!gameEnd) {
                Time.timeScale = 1.0f;
            }
            pauseOn = false;
        } 
    }

    public void unPause() {
        pauseMenu.SetActive(false);
            if (!gameEnd) {
                Time.timeScale = 1.0f;
            }
        pauseOn = false;        
    }

    public Order[] getOrders() {
        return this.orders;
    }

    public int[] getOrderItemIDs(Order order) {
        int[] OrderItemIDs = new int[order.orderItems.Length];
        for (int i = 0; i < order.orderItems.Length; i++) {
            OrderItemIDs[i] = inventoryManager.getItemID(order.orderItems[i].GetComponent<Item>().itemName);
        }
        return OrderItemIDs;
    }

    public int compareOrderAndSupplied() {
        int differentItems = 0;
        int[] supply = inventoryManager.getOrderItemIDs();
        int[] order = this.getOrderItemIDs(this.selectedOrder);
    
        var diff = supply.Except(order).ToArray();

        for (int i = 0; i < supply.Length; i++) {
            int peen = supply[i];
            for (int j = 0; j < diff.Length;j++) {
                if (peen == diff[j]) {
                    differentItems++;
                }
            }
        }
        return differentItems;
    }


    public void AddScore(float points)
    {
        score += points;
        UpdateScoreUI();
    }
    public void minusScore(float points) {
        score-=points;
        UpdateScoreUI();
    }
    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Money: $" + score;
        }
    }
    public void randomizeOrder(Order order) {
        for (int i = 0; i < orderIcons.Length; i++) {
            order.orderItems[i] = Items[Random.Range(0, Items.Length-1)];
        }
    }

    public void randomizeOrders() {
        for (int i = 0; i < orders.Length; i++) {
            if (orders[i] != null) {
                randomizeOrder(orders[i]);
            }
        }
    }

    public void updateTopBar(Order order) {
        if (waitingForOrder) {
            return;
        }
            for (int i = 0; i < orderIcons.Length; i++) {
                orderIcons[i].sprite = order.orderItems[i].GetComponent<Item>().sprite;
            }        
    }

    public void calculateAndGiveScore() {
        float baseScore = (float)this.compareOrderAndSupplied();
        if (baseScore < 2) {
            this.AddScore(100/(baseScore+1));
        }
    }

    public void sendOrder() {
        calculateAndGiveScore();
        notifier.Notify("Order sent");
        orders[this.currentOrderIndex] = null;
        this.orderTimer.timers[this.currentOrderIndex] = -1;

        bool looking = true;
        for (int j = 0; j < orders.Length; j++) {
            if (orders[j] != null) {
                while (looking) {
                    this.selectedOrder = orders[j];
                    this.orderTimer.timerIndex = j;
                    this.currentOrderIndex = j;
                    looking = false;
                }
            }
        }  
        if (looking) {
            for (int i = 0; i < orderIcons.Length; i++) {
                orderIcons[i].sprite = emptySprite;
            }
            disableButtons = true;
            waitingForOrder = true;
        }
        
        inventoryManager.clearOrderSlots();
        this.selectedOrder.orderTime = this.orderTime;


        playerControl.canMove = true;
        updateTopBar(this.selectedOrder);
        topText.text = "Order: " + (this.currentOrderIndex + 1);
        inventoryManager.InventoryMenu.SetActive(false);
        inventoryManager.menuActivated = false;
        if(inventoryManager.orderMenuActivated) {
            inventoryManager.OrderMenu.SetActive(false);
            inventoryManager.orderMenuActivated = false;                
        }
    }

    public void switchSelectedOrderUp() {
        if (!disableButtons) {
            this.currentOrderIndex += 1;
            if (this.currentOrderIndex >= this.orders.Length) {
                this.currentOrderIndex = 0;
            }
            if ( this.currentOrderIndex < 0) {
                this.currentOrderIndex = this.orders.Length - 1;
            }
            if (this.orders[this.currentOrderIndex] == null) {
                switchSelectedOrderUp();
                return;
            }
            this.selectedOrder = this.orders[this.currentOrderIndex];
            topText.text = "Order: " + (this.currentOrderIndex + 1);
            updateTopBar(this.selectedOrder);
            orderTimer.switchSelectedTimeUp();
        }
    }

    public void switchSelectedOrderDown() {
        if (!disableButtons) {
            this.currentOrderIndex -= 1;
            if (this.currentOrderIndex >= this.orders.Length) {
                this.currentOrderIndex = 0;
            }
            if ( this.currentOrderIndex < 0) {
                this.currentOrderIndex = this.orders.Length - 1;
            }
            if (this.orders[this.currentOrderIndex] == null) {
                switchSelectedOrderDown();
                return;
            }
            this.selectedOrder = this.orders[this.currentOrderIndex];
            topText.text = "Order: " + (this.currentOrderIndex + 1);
            updateTopBar(this.selectedOrder);
            orderTimer.switchSelectedTimeDown();
        }
    }

    public void endTimer(bool isShift, bool isOrderCome, int i) {
        if (isShift) {
            endMenu.SetActive(true);
            Time.timeScale = 0.0f;
            gameEnd = true;
            return;
        }
        if (isOrderCome) {
            bool looking = true;
            for (int j = 0; j < orders.Length; j++) {
                if (orders[j] == null) {
                    while (looking) {
                        orders[j] = new Order(this.orderTime, basePenaltyPoints);
                        orders[j].orderItems = new GameObject[orderIcons.Length];
                        orderTimer.addTimer(j, orders[j].orderTime);
                        randomizeOrder(orders[j]);
                        if(disableButtons || waitingForOrder) {
                            disableButtons = false;
                            waitingForOrder = false;
                            this.selectedOrder = orders[j];
                            this.orderTimer.timerIndex = j;
                            this.currentOrderIndex = j;
                            updateTopBar(this.selectedOrder);
                        }
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

        if(i == this.currentOrderIndex) {
            updateTopBar(this.selectedOrder);
        }

        this.orders[i].orderTime = this.orderTime;
    }
}
