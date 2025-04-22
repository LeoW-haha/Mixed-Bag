using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrl :MonoBehaviour
{
    public float moveSpeed = 5.0f;
    private float startMoveSpeed;
    public float runningSpeed = 7.0f;
    public float totalWeight = 0.0f;
    private bool running;
    private bool tired;
    private GameManager gameManager; 

    private Rigidbody2D rb;
    private float speedX;
    private float speedY;
    private bool facingLeft = false;
    public bool canMove = true;

    //Energy System
    public Image StaminaBar;
    public float Stamina, MaxStamina;
    public float RunCost;
    public float staminaDrain;
    public GameObject failMenu;

    void Start ()
    {
        this.startMoveSpeed = this.moveSpeed;
        rb = GetComponent<Rigidbody2D>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update ()
    {
        this.totalWeight = gameManager.totalWeight;
        if (this.totalWeight > gameManager.maxWeight) {
            moveSpeed = startMoveSpeed/2;
            tired = true;
        } else {
            moveSpeed = startMoveSpeed;
            tired = false;
        }

        speedX = Input.GetAxisRaw("Horizontal");
        speedY = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown("left shift") && canMove) {
            running = true;
        } else if (Input.GetKeyUp("left shift")) {
            running = false;
        }

        if(Input.GetAxisRaw("Horizontal") > 0 && facingLeft  && canMove) {
            Flip();
            facingLeft = false;
        }
        if(Input.GetAxisRaw("Horizontal") < 0 && !facingLeft  && canMove) {
            Flip();
            facingLeft = true;
        }

        Stamina -= staminaDrain*Time.deltaTime;
        StaminaBar.fillAmount = Stamina/MaxStamina;
        if (Stamina <= 0) {
            failMenu.SetActive(true);
            failMenu.GetComponent<failMenuController>().changeText("Ran out of energy");
            Time.timeScale = 0.0f;
            gameManager.gameEnd = true;      
        }
    }

    void FixedUpdate ()
    {
        if (running && Stamina>0 && !tired && canMove) {
            rb.linearVelocity = new Vector2(speedX * runningSpeed, speedY * runningSpeed);
            Stamina -= RunCost* Time.deltaTime;
        } else if (canMove) {
            rb.linearVelocity = new Vector2(speedX * moveSpeed, speedY * moveSpeed);
        }
    }

    private void Flip() {
        var theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    
    public void regenStamina(float amount) {
        Stamina += amount;
    }

    public bool isStaminaFull() {
        if (Stamina >= MaxStamina) {
            return true;
        } else {
            return false;
        }
    }

}
