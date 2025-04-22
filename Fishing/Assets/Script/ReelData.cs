using System;
using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(fileName = "NewReel", menuName = "Fishing/Reel Data")]
public class ReelData : ScriptableObject
{
    public int reelID;
    public float reelSpeed;
    public string reelRarity;
    public string reelDesc;

    public string itemKey
        => "reel" + (reelID-10);
    public string reelName
        => LocalizationSettings.StringDatabase.GetLocalizedString(ItemConstants.ItemTable, itemKey);
}
