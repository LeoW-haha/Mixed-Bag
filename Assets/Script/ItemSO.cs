using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public int id;
    public float staminaAmount;
    public int orderAmount;
    public float weight;
    public itemType ItemType = new itemType();

    public bool UseItem()
    {
        var player = GameObject.Find("Player").GetComponent<PlayerCtrl>();

        switch (ItemType)
        {
            case itemType.staminaConsumable:
                if (!player.isStaminaFull())
                {
                    player.regenStamina(staminaAmount);
                    return true;
                }
                return false;

            case itemType.orderItem:
                Debug.Log(this.itemName);
                Debug.Log(this.id);
                return false;

            // Remove or ignore slotUpgrade entirely:
            case itemType.slotUpgrade:
                Debug.LogWarning("slotUpgrade item used but no slot unlocking logic exists.");
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
        slotUpgrade
    };
}
