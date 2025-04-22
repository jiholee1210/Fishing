using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(fileName = "NewWire", menuName = "Fishing/Wire Data")]
public class WireData : ScriptableObject
{
    public int wireID;
    public float wirePower;
    public string wireRarity;
    public string wireDesc;

    public string itemKey
        => "wire" + (wireID-20);
    public string wireName
        => LocalizationSettings.StringDatabase.GetLocalizedString(ItemConstants.ItemTable, itemKey);
}
