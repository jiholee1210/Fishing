using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(fileName = "NewHook", menuName = "Fishing/Hook Data")]
public class HookData : ScriptableObject
{
    public int hookID ;
    public float hookPower;
    public string hookRarity;
    public string hookDesc;

    public string itemKey
        => "hook" + (hookID-30);
    public string hookName
        => LocalizationSettings.StringDatabase.GetLocalizedString(ItemConstants.ItemTable, itemKey);
}
