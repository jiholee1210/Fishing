using UnityEngine;

[CreateAssetMenu(fileName = "NewWire", menuName = "Fishing/Wire Data")]
public class WireData : ScriptableObject
{
    public string wireName;
    public int wireID;
    public float wirePower;
    public string wireRarity;
    public string wireDesc;
}
