using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public int id;
    public int orderAmount;
    public itemType ItemType = new itemType();

    public bool UseItem()
    {
        var player = GameObject.Find("Player").GetComponent<PlayerCtrl>();

        switch (ItemType)
        {
            

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


    public enum itemType {
        orderItem,
        slotUpgrade
    };
}
