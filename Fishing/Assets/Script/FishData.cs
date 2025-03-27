using UnityEngine;

public enum Habitat {
    None,
    Sea,
    Freshwater,
    Rock,
    Lava
}

public enum Rarity {
    일반,
    희귀,
    에픽,
    전설,
    보물
}
[CreateAssetMenu(fileName = "NewFish", menuName = "Fishing/Fish Data")]
public class FishData : ScriptableObject
{
    public string fishName;
    public int fishID;
    public Habitat habitat;
    public Rarity rarity;
    public float power;
    public float speed;
    public float weightMin;
    public float weightMax;
    public string desc;
    public float price;
    public Sprite fishDetail;
    public Sprite fishIcon;
}
