using UnityEngine;

[CreateAssetMenu(fileName = "NewHook", menuName = "Fishing/Hook Data")]
public class HookData : ScriptableObject
{
    public string hookName;
    public int hookID;
    public float hookPower;
    public string hookRarity;
    public string hookDesc;
}
