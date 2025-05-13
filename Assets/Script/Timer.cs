using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    public float[] timers;
    public int timerIndex;
    private float remainingTime;
    public bool isShift;
    public bool isOrderCome;
    private bool past10 = false;
    private bool past5 = false;
    private GameManager gameManager;
    private Notifier notifier;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        notifier = GameObject.Find("NotificationHolder").GetComponent<Notifier>();
        if (timers.Length > 0)
        {
            remainingTime = timers[0];
        }
        timerIndex = 0;
    }

    public void addTimer(int index, float time)
    {
        bool firstAdd = false;
        if (timers[0] == 0)
        {
            firstAdd = true;
        }
        if (timers.Length >= index + 1)
        {
            timers[index] = time;
            if (firstAdd)
            {
                this.remainingTime = timers[0];
            }
        }
    }

    public void switchSelectedTimeUp()
    {
        if (this.timers[this.timerIndex] != -1)
        {
            this.timers[this.timerIndex] = this.remainingTime;
        }
        this.timerIndex += 1;
        if (this.timerIndex >= this.timers.Length)
        {
            this.timerIndex = 0;
        }
        if (this.timerIndex < 0)
        {
            this.timerIndex = this.timers.Length - 1;
        }
        if (this.timers[timerIndex] == -1)
        {
            switchSelectedTimeUp();
            return;
        }
        this.remainingTime = this.timers[this.timerIndex];
        updateTimerText();
    }

    public void switchSelectedTimeDown()
    {
        if (this.timers[this.timerIndex] != -1)
        {
            this.timers[this.timerIndex] = this.remainingTime;
        }
        this.timerIndex -= 1;
        if (this.timerIndex >= this.timers.Length)
        {
            this.timerIndex = 0;
        }
        if (this.timerIndex < 0)
        {
            this.timerIndex = this.timers.Length - 1;
        }
        if (this.timers[timerIndex] == -1)
        {
            switchSelectedTimeDown();
            return;
        }
        this.remainingTime = this.timers[this.timerIndex];
        updateTimerText();

    }

    // Update is called once per frame
    void Update()
    {
        updateAll();
        if (timerText != null)
        {
            updateTimerText();
        }
        this.remainingTime = timers[timerIndex];
    }
    public float updateTimer(float remainingTime, int i)
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 10.0f && !isOrderCome && !isShift && !past10)
            {
                notifier.Notify("Warning: Order " + (i + 1) + " expires in 10 seconds");
                past10 = true;
            }
            if (remainingTime <= 5.0f && !isOrderCome && !isShift && !past5)
            {
                notifier.Notify("Warning: Order " + (i + 1) + " expires in 5 seconds");
                past5 = true;
            }
            return remainingTime;
        }
        else if (remainingTime != -1)
        {
            remainingTime = 0;
            gameManager.endTimer(isShift, isOrderCome, i);
            remainingTime = resetTime(i);
            return remainingTime;
        }
        else
        {
            return -1;
        }


    }
    public void updateTimerText()
    {
        if (gameManager.waitingForOrder)
        {
            timerText.text = "Waiting for Order";
            return;
        }
        int minutes = Mathf.FloorToInt(this.remainingTime / 60);
        int seconds = Mathf.FloorToInt(this.remainingTime % 60);
        if (isShift)
        {
            timerText.text = "Shift: " + string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            timerText.text = "Shift: " + string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
    public void updateAll()
    {
        for (int i = 0; i < timers.Length; i++)
        {
            if (timers[i] != -1)
            {
                timers[i] = updateTimer(timers[i], i);
            }
        }
    }
    public float resetTime(int i)
    {
        timers[i] = gameManager.getOrders()[i].orderTime;
        if (isOrderCome)
        {
            timers[i] = gameManager.arriveTime;
        }
        if (i == timerIndex)
        {
            this.remainingTime = gameManager.getOrders()[i].orderTime;
            if (isOrderCome)
            {
                this.remainingTime = gameManager.arriveTime;
            }
        }
        return timers[i];
    }
}