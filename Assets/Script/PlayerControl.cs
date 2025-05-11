using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    private float startMoveSpeed;
    public float runningSpeed = 7.0f;
    public float totalWeight = 0.0f;
    private bool running;
    private bool tired;
    private GameManager gameManager;
    public string currentPackagingColour = "";

    private Rigidbody2D rb;
    private float speedX;
    private float speedY;
    private bool facingLeft = false;
    public bool canMove = true;

    // Energy System
    public Image StaminaBar;
    public float Stamina, MaxStamina;
    public float RunCost;
    public float staminaDrain;

    void Start()
    {
        startMoveSpeed = moveSpeed;
        rb = GetComponent<Rigidbody2D>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        totalWeight = gameManager.totalWeight;

        if (totalWeight > gameManager.maxWeight)
        {
            moveSpeed = startMoveSpeed / 2;
            tired = true;
        }
        else
        {
            moveSpeed = startMoveSpeed;
            tired = false;
        }

        speedX = Input.GetAxisRaw("Horizontal");
        speedY = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.LeftShift) && canMove)
        {
            running = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            running = false;
        }

        if (Input.GetAxisRaw("Horizontal") > 0 && facingLeft && canMove)
        {
            Flip();
            facingLeft = false;
        }
        if (Input.GetAxisRaw("Horizontal") < 0 && !facingLeft && canMove)
        {
            Flip();
            facingLeft = true;
        }

        Stamina -= staminaDrain * Time.deltaTime;
        Stamina = Mathf.Clamp(Stamina, 0, MaxStamina);

        if (StaminaBar != null)
        {
            StaminaBar.fillAmount = Stamina / MaxStamina;
        }
    }

    void FixedUpdate()
    {
        if (running && Stamina > 0 && !tired && canMove)
        {
            rb.linearVelocity = new Vector2(speedX * runningSpeed, speedY * runningSpeed);
            Stamina -= RunCost * Time.deltaTime;
        }
        else if (canMove)
        {
            rb.linearVelocity = new Vector2(speedX * moveSpeed, speedY * moveSpeed);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void ApplyPaint(string colour)
    {
        currentPackagingColour = colour;

    }


    private void Flip()
    {
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void regenStamina(float amount)
    {
        Stamina += amount;
        Stamina = Mathf.Clamp(Stamina, 0, MaxStamina);
    }

    public bool isStaminaFull()
    {
        return Stamina >= MaxStamina;
    }
}
