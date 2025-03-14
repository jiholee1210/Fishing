using UnityEngine;

public enum Habitat {
    None,
    Sea,
    Freshwater,
    Rock,
    Lava
}
[CreateAssetMenu(fileName = "NewFish", menuName = "Fishing/Fish Data")]
public class FishData : ScriptableObject
{
    public string fishName;
    public int fishID;
    public Habitat habitat;
    public string rarity;
    public float power;
    public float speed;
    public float weightMin;
    public float weightMax;
    public string desc;
    public float price;
    public Sprite fishDetail;
    public Sprite fishIcon;
}
