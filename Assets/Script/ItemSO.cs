using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public int id;
    public float staminaAmount;
    public int orderAmount;
    public int restockAmount;
    public float weight;
    public itemType ItemType = new itemType();

    public bool UseItem() {
        if(ItemType == itemType.staminaConsumable) {
            if (!GameObject.Find("Player").GetComponent<PlayerCtrl>().isStaminaFull()) {
                GameObject.Find("Player").GetComponent<PlayerCtrl>().regenStamina(staminaAmount);
                return true;
            } else {
                return false;
            }
        } if (ItemType == itemType.orderItem) {
            Debug.Log(this.itemName);
            Debug.Log(this.id);
            return false;        
        } if (ItemType == itemType.slotUpgrade) {
            return GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>().unlockSlots(orderAmount);
        }
        if (ItemType == itemType.leakFixer) {
            return false;
        }
        return false;
    }

    public int getItemID() {
        return this.id;
    }

    public float getWeight() {
        return this.weight;
    }

    public enum itemType {
        orderItem,
        staminaConsumable,
        slotUpgrade,
        leakFixer
    };
}
