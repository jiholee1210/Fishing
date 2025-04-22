using System;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType {
    Rod,
    Reel,
    Wire,
    Hook,
    Bait,
    Relic
}

public static class ItemConstants {
    public static readonly string ItemTable = "Item Table";
}

[Serializable]
[CreateAssetMenu(fileName = "NewItem", menuName = "Fishing/Item Data")]
public class ItemData : ScriptableObject
{  
    public int itemID;
    public ItemType itemType;
    public int reqGold;
    public Sprite itemImage;
}
