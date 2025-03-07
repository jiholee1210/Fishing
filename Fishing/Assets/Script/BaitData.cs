using UnityEngine;

[CreateAssetMenu(fileName = "NewBait", menuName = "Fishing/Bait Data")]
public class BaitData : ScriptableObject
{
    public string baitName;
    public int baitID;
    public int baitLevel;
    public string baitRarity;
    public string baitDesc;
}
