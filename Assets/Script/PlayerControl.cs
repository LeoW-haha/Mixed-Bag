using UnityEngine;

public class PlayerCtrl :MonoBehaviour
{
    public float moveSpeed = 5f;

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

        if(Input.GetAxisRaw("Horizontal") > 0 && facingLeft) {
            Flip();
            facingLeft = false;
        }
        if(Input.GetAxisRaw("Horizontal") < 0 && !facingLeft) {
            Flip();
            facingLeft = true;
        }
    }

    void FixedUpdate ()
    {
        rb.linearVelocity = new Vector2(speedX * moveSpeed, speedY * moveSpeed);
    }

    private void Flip() {
        var theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
