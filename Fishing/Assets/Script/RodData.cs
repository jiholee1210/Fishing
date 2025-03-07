using UnityEngine;

[CreateAssetMenu(fileName = "NewRod", menuName = "Fishing/Rod Data")]
public class RodData : ScriptableObject
{
    public string rodName;
    public int rodID;
    public float rodDur;
    public string rodRarity;
    public string rodDesc;
    public GameObject rodPrefab;
}
