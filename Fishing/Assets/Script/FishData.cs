using UnityEngine;
using UnityEngine.Localization.Settings;

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
public static class FishConstants {
    public static readonly string FishTable = "Fish Table";
}

[CreateAssetMenu(fileName = "NewFish", menuName = "Fishing/Fish Data")]
public class FishData : ScriptableObject
{
    
    public int fishID;
    public Habitat habitat;
    public Rarity rarity;
    public float power;
    public float speed;
    public float weightMin;
    public float weightMax;
    public float price;
    public Sprite fishDetail;
    public Sprite fishIcon;

    public string fishNameKey
        => "name" + fishID;
    public string fishDescKey
        => "desc" + fishID;
    public string fishRarityKey
        => "rarity" + (int)rarity;
    public string fishName
        => LocalizationSettings.StringDatabase.GetLocalizedString(FishConstants.FishTable, fishNameKey);
    public string desc
        => LocalizationSettings.StringDatabase.GetLocalizedString(FishConstants.FishTable, fishDescKey);
    public string rarityLocalized
        => LocalizationSettings.StringDatabase.GetLocalizedString(FishConstants.FishTable, fishRarityKey);
}
