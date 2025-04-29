using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(fileName = "NewBait", menuName = "Fishing/Bait Data")]
public class BaitData : ScriptableObject
{
    public int baitID;
    public int baitLevel;
    public string baitRarity;
    public string baitDesc;

    public string itemKey
        => "bait" + (baitID-40);
    public string baitName
        => LocalizationSettings.StringDatabase.GetLocalizedString(ItemConstants.ItemTable, itemKey);
}
