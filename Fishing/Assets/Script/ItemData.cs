using System;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType {
    Rod,
    Reel,
    Wire,
    Hook,
    Bait
}
[Serializable]
[CreateAssetMenu(fileName = "NewItem", menuName = "Fishing/Item Data")]
public class ItemData : ScriptableObject
{  
    public int itemID;
    public ItemType itemType;
    public Sprite itemImage;

    public ItemData Clone() {
        ItemData clone = ScriptableObject.CreateInstance<ItemData>();
        clone.itemID = itemID;
        clone.itemType = itemType;
        clone.itemImage = itemImage;

        return clone;
    }
}
