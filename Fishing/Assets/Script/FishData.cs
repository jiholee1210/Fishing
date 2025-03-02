using UnityEngine;

[CreateAssetMenu(fileName = "NewFish", menuName = "Fishing/Fish Data")]
public class FishData : ScriptableObject
{
    public string fishName;
    public int fishID;
    public string rarity;
    public float power;
    public float weightMin;
    public float weightMax;
    public string desc;
    public float price;
    public GameObject fishPrefab;
    public Sprite fishIcon;
}
