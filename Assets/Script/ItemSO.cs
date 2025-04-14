using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public int id;
    public int staminaAmount;
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
        } 
        return false;
    }

    public int getItemID() {
        return this.id;
    }

    public enum itemType {
        orderItem,
        staminaConsumable
    };
}
