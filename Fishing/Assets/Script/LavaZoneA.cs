using System.Collections.Generic;
using UnityEngine;

public class LavaZoneA : MonoBehaviour, IFishingZone
{
    private List<FishData> fishList;

    public List<FishData> GetFishList()
    {
        return fishList;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fishList = new()
        {
            DataManager.Instance.GetFishData(2),
            DataManager.Instance.GetFishData(19),
            DataManager.Instance.GetFishData(20),
            DataManager.Instance.GetFishData(21)
        };
    }
}
