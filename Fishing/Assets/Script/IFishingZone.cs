using System.Collections.Generic;
using UnityEngine;

public interface IFishingZone
{
    List<FishData> GetFishList();
    List<int> GetItemList();
} 
