using System.Collections.Generic;
using UnityEngine;

public interface IFishingZone : IScannable
{
    List<FishData> GetFishList();
} 
