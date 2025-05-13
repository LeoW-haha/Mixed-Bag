using UnityEngine;

public class FuelStationTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[FuelStationTrigger] Player entered fuel station trigger.");

            PlayerFuel playerFuel = other.GetComponent<PlayerFuel>();
            FuelRequestSpawner fuelRequest = FindObjectOfType<FuelRequestSpawner>();

            if (fuelRequest == null)
            {
                Debug.LogWarning("[FuelStationTrigger] No FuelRequestDisplay found in scene.");
                return;
            }

            if (playerFuel == null)
            {
                Debug.LogWarning("[FuelStationTrigger] No PlayerFuel component found on player.");
                return;
            }

            string playerFuelTag = playerFuel.currentFuelTag;
            string requestedFuelTag = fuelRequest.GetRequiredFuelTag();

            Debug.Log($"[FuelStationTrigger] Player is carrying fuel: {playerFuelTag}");
            Debug.Log($"[FuelStationTrigger] Fuel requested: {requestedFuelTag}");

            if (string.IsNullOrEmpty(playerFuelTag))
            {
                Debug.Log("[FuelStationTrigger] Player has no fuel equipped.");
                return;
            }

            if (playerFuelTag == requestedFuelTag)
            {
                Debug.Log("[FuelStationTrigger] Correct fuel delivered!");
                fuelRequest.DeliverFuel(playerFuelTag);
            }
            else
            {
                Debug.Log("[FuelStationTrigger] Wrong fuel type. Delivery rejected.");
            }
        }
    }
}
