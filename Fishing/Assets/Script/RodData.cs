using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(fileName = "NewRod", menuName = "Fishing/Rod Data")]
public class RodData : ScriptableObject
{
    public int rodID;
    public float rodDur;
    public string rodRarity;
    public string rodDesc;
    public GameObject rodPrefab;

    public string itemKey
        => "rod" + rodID;
    public string rodName
        => LocalizationSettings.StringDatabase.GetLocalizedString(ItemConstants.ItemTable, itemKey);
}
