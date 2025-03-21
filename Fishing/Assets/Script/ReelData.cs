using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewReel", menuName = "Fishing/Reel Data")]
public class ReelData : ScriptableObject
{
    public string reelName;
    public int reelID;
    public float reelSpeed;
    public string reelRarity;
    public string reelDesc;
}
