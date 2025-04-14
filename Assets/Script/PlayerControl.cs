using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrl :MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float runningSpeed = 7.0f;
    private bool running;

    private Rigidbody2D rb;
    private float speedX;
    private float speedY;
    private bool facingLeft = false;

    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update ()
    {
        speedX = Input.GetAxisRaw("Horizontal");
        speedY = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown("left shift")) {
            running = true;
        } else if (Input.GetKeyUp("left shift")) {
            running = false;
        }

        if(Input.GetAxisRaw("Horizontal") > 0 && facingLeft) {
            Flip();
            facingLeft = false;
        }
        if(Input.GetAxisRaw("Horizontal") < 0 && !facingLeft) {
            Flip();
            facingLeft = true;
        }

        Stamina -= staminaDrain*Time.deltaTime;
        StaminaBar.fillAmount = Stamina/MaxStamina;
        if (Stamina <= 0) {
            Stamina = 0;
        }
    }

    void FixedUpdate ()
    {
        if (running && Stamina>0) {
            rb.linearVelocity = new Vector2(speedX * runningSpeed, speedY * runningSpeed);
            Stamina -= RunCost* Time.deltaTime;
        } else {
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
    //Energy System
    public Image StaminaBar;
    public float Stamina, MaxStamina;
    public float RunCost;
    public float staminaDrain;
}
