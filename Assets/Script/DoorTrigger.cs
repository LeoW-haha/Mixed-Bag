using UnityEngine;

public class DoorTrigger2D : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {

            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.AddScore();
            }

        }
    }
}
