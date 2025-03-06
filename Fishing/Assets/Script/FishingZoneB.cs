using System.Collections.Generic;
using UnityEngine;

public class FishingZoneB : MonoBehaviour, IFishingZone
{
    private List<FishData> fishList;

    public List<FishData> GetFishList()
    {
        return fishList;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fishList = new() {
            DataManager.Instance.GetFishData(1),
            DataManager.Instance.GetFishData(2),
            DataManager.Instance.GetFishData(4),
            DataManager.Instance.GetFishData(5),
            DataManager.Instance.GetFishData(8),
            DataManager.Instance.GetFishData(9),
            DataManager.Instance.GetFishData(10),
            DataManager.Instance.GetFishData(11)
        };
    }
}
