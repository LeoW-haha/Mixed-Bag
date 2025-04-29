using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public string description;
    [SerializeField] private int baseValue = 100;

    public int GetValue()
    {
        return baseValue;
    }
} 