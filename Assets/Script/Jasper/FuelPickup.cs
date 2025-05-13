using UnityEngine;

public class FuelPickup : MonoBehaviour
{
    public string fuelTag;            // Set to "Red", "Green", etc. in Inspector
    public Sprite fuelSprite;         // Optional: used for UI

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[FuelPickup] Player touched fuel: {fuelTag}");

            PlayerFuel playerFuel = other.GetComponent<PlayerFuel>();
            if (playerFuel != null)
            {
                playerFuel.EquipFuel(fuelTag);
                Debug.Log($"[FuelPickup] Assigned fuel to player: {fuelTag}");
            }
        }
    }


// Update is called once per frame
void Update()
    {
        
    }
}
