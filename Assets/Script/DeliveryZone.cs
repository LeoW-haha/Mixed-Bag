using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    public int deliveryZoneNumber = 1; // Set to 1 or 2 in the Inspector

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerCtrl player = collision.GetComponent<PlayerCtrl>();
        if (player != null && player.hasPackage)
        {
            InventoryManager inventory = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
            GameObject package = inventory.currentPackage;

            if (package != null)
            {
                // Detach package
                package.transform.position = transform.position;

                // Stop it from following the player
                PackageFollower pf = package.GetComponent<PackageFollower>();
                if (pf != null)
                {
                    Destroy(pf);
                }

                // Verify delivery contents
                DeliveryPackage deliveryData = package.GetComponent<DeliveryPackage>();
                GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();

                if (deliveryData != null)
                {
                    gm.VerifyDelivery(deliveryData); // ✅ Compare with order
                }

                // Hand to conveyor
                gm.SendToDeliveryConveyor(package, deliveryZoneNumber);

                // Clear state
                player.hasPackage = false;
                inventory.currentPackage = null;
            }
        }
    }
}
