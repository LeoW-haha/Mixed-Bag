using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {

            GameManager gm = FindFirstObjectByType<GameManager>(); 
            if (gm != null)
            {
                if (gameObject.CompareTag("SmallDoor"))
                {
                    gm.AddScore(1);
                    Debug.Log("Small door: +1 point");
                }
                else if (gameObject.CompareTag("BigDoor"))
                {
                    gm.AddScore(2);
                    Debug.Log("Big door: +2 points");
                }
            }

        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
