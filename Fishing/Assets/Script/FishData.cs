using UnityEngine;

[CreateAssetMenu(fileName = "NewFish", menuName = "Fishing/Fish Data")]
public class FishData : ScriptableObject
{
    public string fishName;
    public int fishID;
    public int rarity;
    public float power;
    public GameObject fishPrefab;
    public Sprite fishIcon;
}
