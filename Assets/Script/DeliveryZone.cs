using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    private InventoryManager inventoryManager;

    public Transform deliveryWaypoint1;
    public Transform deliveryWaypoint2;

    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerCtrl player = collision.GetComponent<PlayerCtrl>();

        if (player != null && player.hasPackage)
        {
            GameObject package = inventoryManager.currentPackage;

            if (package == null)
            {
                Debug.LogWarning("❌ No current package found in inventory.");
                return;
            }

            // Detach package from player
            package.transform.position = transform.position;

            // Disable PackageFollower
            PackageFollower follower = package.GetComponent<PackageFollower>();
            if (follower != null)
            {
                follower.enabled = false;
            }

            // Enable ConveyorMover and assign path
            ConveyorMover mover = package.GetComponent<ConveyorMover>();
            if (mover != null && deliveryWaypoint1 != null && deliveryWaypoint2 != null)
            {
                mover.enabled = true;
                mover.SetPath(deliveryWaypoint1, deliveryWaypoint2);
            }
            else
            {
                Debug.LogWarning("❌ Missing ConveyorMover or Waypoints.");
            }

            // Reset player and inventory state
            player.hasPackage = false;
            inventoryManager.currentPackage = null;

            Debug.Log("✅ Package delivered and attached to conveyor.");
        }
    }
}
