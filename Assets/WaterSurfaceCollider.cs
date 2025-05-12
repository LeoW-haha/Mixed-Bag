using UnityEngine;

public class WaterSurfaceCollider : MonoBehaviour
{
    [SerializeField] private float slowDown;
    private PlayerController player;
    private float startingSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        if (player != null) {
            startingSpeed = player.getSpeed();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision) 
    { 
        if (collision.gameObject.CompareTag("Player")) 
        { 
            player.changeSpeed(slowDown); 
        } 
    } 
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player.changeSpeed(startingSpeed); 
        }
    }
}
